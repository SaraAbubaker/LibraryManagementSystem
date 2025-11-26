using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        //navigation to the lists
        private readonly LibraryDataStore Store;
        private readonly InventoryService Inventory;
        public BorrowService(LibraryDataStore store, InventoryService inventory)
        {
            Store = store;
            Inventory = inventory;
        }
        public bool IsAnyCopyAvailable(int bookId)
        {
            return Inventory.GetAvailableCopies(bookId).Any();
        }
    }
}
