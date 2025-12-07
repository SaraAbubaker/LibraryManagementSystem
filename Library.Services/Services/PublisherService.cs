using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.Publisher;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Mapster;

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
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, nameof(dto.Name));
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
            publisherDto.InventoryCount = publisher.InventoryRecords.Count;

            return publisherDto;
        }

        public async Task<List<PublisherDto>> GetAllPublishersAsync()
        {
            var publishers = (await _publisherRepo.GetAllAsync())
                .Where(p => !p.IsArchived)
                .ToList();

            var dtos = publishers.Adapt<List<PublisherDto>>();

            for (int i = 0; i < dtos.Count; i++)
            {
                dtos[i].InventoryCount = publishers[i].InventoryRecords.Count;
            }

            return dtos;
        }

        public async Task<PublisherDto> GetPublisherByIdAsync(int id)
        {
            Validate.Positive(id, nameof(id));

            var publisher = await _publisherRepo.GetByIdAsync(id);

            var dto = publisher.Adapt<PublisherDto>();
            dto.InventoryCount = publisher.InventoryRecords?.Count ?? 0;

            return dto;
        }
        
        public async Task<PublisherDto> UpdatePublisherAsync(UpdatePublisherDto dto, int userId, int publisherId)
        {
            Validate.NotNull(dto, nameof(dto)); 
            Validate.NotEmpty(dto.Name, nameof(dto.Name));
            Validate.Positive(publisherId, nameof(publisherId));
            Validate.Positive(userId, nameof(userId));

            var publisher = await _publisherRepo.GetByIdAsync(publisherId);

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

            var publisher = await _publisherRepo.GetByIdAsync(id);

            if (publisher.IsArchived)
                throw new ConflictException($"Publisher with id {id} is already archived.");

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
