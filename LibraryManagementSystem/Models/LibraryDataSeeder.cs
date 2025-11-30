using System.Net;
using System.Reflection.Emit;

namespace LibraryManagementSystem.Entities
{
    public static class LibraryDataSeeder
    {
        public static void Seed(LibraryDataStore store)
        {
            store.Authors = [
                new Author { Id = 1, Name = "J.K. Rowling" },
                new Author { Id = 2, Name = "George Orwell" },
                new Author { Id = 3, Name = "Haruki Murakami" }
            ];

            store.Categories = [
                new Category { Id = 1, Name = "Fiction" },
                new Category { Id = 2, Name = "Fantasy" },
                new Category { Id = 3, Name = "Science" }
            ];

            store.Books = [
                new Book
                {
                    Id = 1,
                    Title = "1984",
                    AuthorId = 2, // George Orwell
                    PublishDate = new DateTime(1949, 6, 8),
                    Publisher = "Secker & Warburg",
                    Version = "1"
                },
                new Book
                {
                    Id = 2,
                    Title = "Norwegian Wood",
                    AuthorId = 3, // Haruki Murakami
                    PublishDate = new DateTime(1987, 9, 4),
                    Publisher = "Kodansha",
                    Version = "1"
                },
                new Book
                {
                    Id = 3,
                    Title = "Harry Potter and the Philosopher's Stone",
                    AuthorId = 1, // J.K. Rowling
                    PublishDate = new DateTime(1997, 6, 26),
                    Publisher = "Bloomsbury",
                    Version = "1"
                }
            ];

            store.InventoryRecords = [
                // Book 1 (1984)
                new InventoryRecord { Id = 1, BookId = 1, CopyCode = "1984-01", IsAvailable = true },
                new InventoryRecord { Id = 2, BookId = 1, CopyCode = "1984-02", IsAvailable = true },

                // Book 2 (Norwegian Wood)
                new InventoryRecord { Id = 3, BookId = 2, CopyCode = "NW-01", IsAvailable = true },
                new InventoryRecord { Id = 4, BookId = 2, CopyCode = "NW-02", IsAvailable = true },

                // Book 3 (Harry Potter)
                new InventoryRecord { Id = 5, BookId = 3, CopyCode = "HP-01", IsAvailable = true },
                new InventoryRecord { Id = 6, BookId = 3, CopyCode = "HP-02", IsAvailable = true }
            ];




        }
    }
}
