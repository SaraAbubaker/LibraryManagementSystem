using LibraryManagementSystem;
using LibraryManagementSystem.Entities;

class Program
{
    static void Main()
    {
        var store = new LibraryDataStore();
        LibraryDataSeeder.Seed(store);

        Console.WriteLine($"Authors: {store.Authors.Count}");
        Console.WriteLine($"Categories: {store.Categories.Count}");
        
    }
}
