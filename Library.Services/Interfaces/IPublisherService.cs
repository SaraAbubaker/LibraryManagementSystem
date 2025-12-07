
using Library.Shared.DTOs.Publisher;

namespace Library.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto dto, int createdByUserId);
        Task<List<PublisherDto>> GetAllPublishersAsync();
        Task<PublisherDto> GetPublisherByIdAsync(int id);
        Task<PublisherDto> UpdatePublisherAsync(UpdatePublisherDto dto, int userId, int publisherId);
        Task<bool> ArchivePublisherAsync(int id, int archivedByUserId);
    }
}
