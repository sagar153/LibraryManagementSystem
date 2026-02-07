namespace LibraryManagementSystem.Services.Interfaces;

using Models;

/// <summary>
/// Interface for book service operations
/// </summary>
public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync();
    
    Task<Book?> GetBookByIdAsync(int id);
    
    Task<List<Book>> GetBooksByCategoryAsync(string category);
    
    Task<List<Book>> SearchBooksAsync(string searchTerm);
    
    Task<Book> CreateBookAsync(Book book);
    
    Task<Book?> UpdateBookAsync(int id, Book book);
    
    Task<bool> DeleteBookAsync(int id);
    
    Task<bool> UpdateAvailableCopiesAsync(int bookId, int quantity);
}
