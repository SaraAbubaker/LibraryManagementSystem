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
        public PublisherDto CreatePublisher(CreatePublisherDto dto, int createdByUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Publisher name");

            var publisher = dto.Adapt<Publisher>();
            publisher.CreatedByUserId = createdByUserId;
            publisher.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            publisher.IsArchived = false;

            //repository handles savechanges 
            _publisherRepo.Add(publisher); 

            var publisherDto = publisher.Adapt<PublisherDto>();
            publisherDto.InventoryCount = publisher.InventoryRecords.Count;

            return publisherDto;
        }

        public List<PublisherDto> GetAllPublishers()
        {
            var publishers = _publisherRepo.GetAll()
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

            var publisher = _publisherRepo.GetById(id);

            var dto = publisher!.Adapt<PublisherDto>();
            dto.InventoryCount = publisher!.InventoryRecords.Count;

            return dto;
        }

        public PublisherDto UpdatePublisher(UpdatePublisherDto dto, int userId, int publisherId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Publisher name");
            Validate.Positive(publisherId, "Publisher id");

            var publisher = _publisherRepo.GetById(publisherId);

            publisher!.Name = dto.Name;
            publisher.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            publisher.LastModifiedByUserId = userId;

            _publisherRepo.Update(publisher);

            var publisherDto = publisher.Adapt<PublisherDto>();
            publisherDto.InventoryCount = publisher.InventoryRecords.Count;

            return publisherDto;
        }

        public bool ArchivePublisher(int id, int? archivedByUserId = null)
        {
            var publisher = _publisherRepo.GetById(id);

            if (publisher.IsArchived)
                throw new ConflictException($"Publisher with id {id} is already archived.");

            publisher.IsArchived = true;
            publisher.ArchivedByUserId = archivedByUserId;
            publisher.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _publisherRepo.Update(publisher);

            return true;
        }
    }
}
