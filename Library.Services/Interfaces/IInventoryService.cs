using Library.Entities.Models;
using Library.Shared.DTOs.InventoryRecord;

namespace Library.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<bool> ReturnCopyAsync(int inventoryRecordId, int currentUserId);
        Task<InventoryRecord> CreateCopyAsync(int bookId, string copyCode, int createdByUserId);
        Task<bool> RemoveCopyAsync(int inventoryRecordId, int performedByUserId);
        Task<List<InventoryRecord>> ListCopiesForBookAsync(int bookId);
        Task<List<InventoryRecord>> GetAvailableCopiesAsync(int bookId);
    }
}
