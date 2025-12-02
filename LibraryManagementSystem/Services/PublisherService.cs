using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.DTOs.Publisher;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class PublisherService
    {
        private readonly LibraryContext _context;

        public PublisherService(LibraryContext context)
        {
            _context = context;
        }

        //CRUD
        public PublisherDto CreatePublisher(CreatePublisherDto dto, int createdByUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Publisher name");

            var publisher = dto.Adapt<Publisher>();
            publisher.CreatedByUserId = createdByUserId;
            publisher.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            publisher.IsArchived = false;

            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            var publisherDto = publisher.Adapt<PublisherDto>();
            publisherDto.InventoryCount = publisher.InventoryRecords.Count;

            return publisherDto;
        }

        public List<PublisherDto> GetAllPublishers()
        {
            var publishers = _context.Publishers
                .Include(p => p.InventoryRecords)
                .Where(p => !p.IsArchived)
                .ToList();

            var dtos = publishers.Adapt<List<PublisherDto>>();

            for (int i = 0; i < dtos.Count; i++)
            {
                dtos[i].InventoryCount = publishers[i].InventoryRecords.Count;
            }

            return dtos;
        }

        public PublisherDto GetPublisherById(int id)
        {
            Validate.Positive(id, "Id");

            var publisher = _context.Publishers
                .Include(p => p.InventoryRecords)
                .FirstOrDefault(p => p.Id == id);

            Validate.Exists(publisher, $"Publisher with id {id}");

            var dto = publisher!.Adapt<PublisherDto>();
            dto.InventoryCount = publisher!.InventoryRecords.Count;

            return dto;
        }

        public PublisherDto UpdatePublisher(UpdatePublisherDto dto, int userId, int publisherId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Publisher name");
            Validate.Positive(publisherId, "Publisher id");

            var publisher = _context.Publishers.FirstOrDefault(p => p.Id == publisherId);
            Validate.Exists(publisher, $"Publisher with id {publisherId}");

            publisher!.Name = dto.Name;
            publisher.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            publisher.LastModifiedByUserId = userId;

            _context.SaveChanges();

            var publisherDto = publisher.Adapt<PublisherDto>();
            publisherDto.InventoryCount = publisher.InventoryRecords.Count;

            return publisherDto;
        }

        public bool ArchivePublisher(int id, int? archivedByUserId = null)
        {
            Validate.Positive(id, "Id");

            var publisher = _context.Publishers.FirstOrDefault(p => p.Id == id);
            Validate.Exists(publisher, $"Publisher with id {id}");

            if (publisher!.IsArchived)
                throw new ConflictException($"Publisher with id {id} is already archived.");

            publisher.IsArchived = true;
            publisher.ArchivedByUserId = archivedByUserId;
            publisher.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.SaveChanges();

            return true;
        }
    }
}
