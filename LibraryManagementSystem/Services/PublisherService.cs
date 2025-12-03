using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.DTOs.Publisher;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class PublisherService
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
            Validate.NotEmpty(dto.Name, "Publisher name");

            var publisher = new Publisher
            {
                Name = dto.Name,
                CreatedByUserId = createdByUserId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
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
            Validate.Positive(id, "Id");

            var publisher = await _publisherRepo.GetByIdAsync(id);

            var dto = publisher.Adapt<PublisherDto>();
            dto.InventoryCount = publisher.InventoryRecords?.Count ?? 0;

            return dto;
        }

        public async Task<PublisherDto> UpdatePublisherAsync(UpdatePublisherDto dto, int userId, int publisherId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Publisher name");
            Validate.Positive(publisherId, "Publisher id");
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
            Validate.Positive(id, "Id");
            Validate.Positive(archivedByUserId, nameof(archivedByUserId));

            var publisher = await _publisherRepo.GetByIdAsync(id);

            if (publisher.IsArchived)
                throw new ConflictException($"Publisher with id {id} is already archived.");

            publisher.IsArchived = true;
            publisher.ArchivedByUserId = archivedByUserId;
            publisher.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            await _publisherRepo.UpdateAsync(publisher);

            return true;
        }
    }
}
