using Library.Entities.Models;
using Library.Shared.DTOs.BorrowRecord;

namespace Library.Services.Interfaces
{
    public interface IBorrowService
    {
        Task<List<BorrowDto>> GetBorrowDetailsAsync();
        Task<bool> HasAvailableCopyAsync(int bookId);
        Task<InventoryRecord?> GetAvailableCopyAsync(int bookId);
        Task<BorrowRecord> BorrowBookAsync(RequestBorrowDto dto, int userId);
        Task<bool> ReturnBookAsync(int borrowRecordId, int currentUserId);
        Task<List<BorrowRecord>> GetOverdueRecordsAsync();
        bool IsBorrowOverdue(BorrowRecord record);
        int CalculateOverdueDays(BorrowRecord record);

    }
}
