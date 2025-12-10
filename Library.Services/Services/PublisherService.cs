using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.Publisher;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IGenericRepository<Publisher> _publisherRepo;

        public PublisherService(IGenericRepository<Publisher> publisherRepo)
        {
            _publisherRepo = publisherRepo;
        }


        //CRUD
        public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto dto, int createdByUserId)
        {
            Validate.ValidateModel(dto); 
            Validate.Positive(createdByUserId, nameof(createdByUserId));

            var publisher = new Publisher
            {
                Name = dto.Name,
                CreatedByUserId = createdByUserId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                LastModifiedByUserId = createdByUserId,
                LastModifiedDate = DateOnly.FromDateTime(DateTime.Now),
                IsArchived = false
            };

            //repository handles savechanges 
            await _publisherRepo.AddAsync(publisher);

            var publisherDto = publisher.Adapt<PublisherDto>();
            publisherDto.InventoryCount = publisher.InventoryRecords?.Count ?? 0;

            return publisherDto;
        }

        public IQueryable<PublisherDto> GetAllPublishersQuery()
        {
            return _publisherRepo.GetAll()
                .Include(p => p.InventoryRecords)
                .AsNoTracking()
                .Select(p => new PublisherDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    InventoryCount = p.InventoryRecords.Count()
                });
        }

        public IQueryable<PublisherDto> GetPublisherByIdQuery(int id)
        {
            Validate.Positive(id, nameof(id));

            return _publisherRepo.GetAll()
                .Include(p => p.InventoryRecords)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PublisherDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    InventoryCount = p.InventoryRecords.Count()
                });
        }
        
        public async Task<PublisherDto> UpdatePublisherAsync(UpdatePublisherDto dto, int userId, int publisherId)
        {
            Validate.ValidateModel(dto);
            Validate.Positive(publisherId, nameof(publisherId));
            Validate.Positive(userId, nameof(userId));

            var publisher = Validate.Exists(
                await _publisherRepo.GetById(publisherId)
                    .Include(p => p.InventoryRecords)
                    .FirstOrDefaultAsync(), 
                publisherId
            );

            publisher.Name = dto.Name;
            publisher.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            publisher.LastModifiedByUserId = userId;

            await _publisherRepo.UpdateAsync(publisher);

            var publisherDto = publisher.Adapt<PublisherDto>();
            publisherDto.InventoryCount = publisher.InventoryRecords?.Count ?? 0;

            return publisherDto;
        }

        public async Task<bool> ArchivePublisherAsync(int id, int archivedByUserId)
        {
            Validate.Positive(id, nameof(id));
            Validate.Positive(archivedByUserId, nameof(archivedByUserId));

            var publisher = Validate.Exists(
                await _publisherRepo.GetById(id)
                    .Include(p => p.InventoryRecords)
                    .FirstOrDefaultAsync(), 
                id
            );

            publisher.IsArchived = true;
            publisher.ArchivedByUserId = archivedByUserId;
            publisher.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            publisher.LastModifiedByUserId = archivedByUserId;
            publisher.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _publisherRepo.UpdateAsync(publisher);

            return true;
        }
    }
}
