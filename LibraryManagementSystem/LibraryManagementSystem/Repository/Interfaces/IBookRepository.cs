namespace LibraryManagementSystem.Repository.Interfaces;

using Models;

/// <summary>
/// Interface for book repository operations
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Get all active books
    /// </summary>
    Task<List<Book>> GetAllBooksAsync();
    
    /// <summary>
    /// Get book by ID
    /// </summary>
    Task<Book?> GetBookByIdAsync(int id);
    
    /// <summary>
    /// Get books by category
    /// </summary>
    Task<List<Book>> GetBooksByCategoryAsync(string category);
    
    /// <summary>
    /// Search books by title, author, or ISBN
    /// </summary>
    Task<List<Book>> SearchBooksAsync(string searchTerm);
    
    /// <summary>
    /// Create new book
    /// </summary>
    Task<Book> CreateBookAsync(Book book);
    
    /// <summary>
    /// Update existing book
    /// </summary>
    Task<Book?> UpdateBookAsync(int id, Book book);
    
    /// <summary>
    /// Delete book (soft delete)
    /// </summary>
    Task<bool> DeleteBookAsync(int id);
    
    /// <summary>
    /// Update available copies
    /// </summary>
    Task<bool> UpdateAvailableCopiesAsync(int bookId, int quantity);
    
    /// <summary>
    /// Check if book exists by ISBN
    /// </summary>
    Task<bool> BookExistsByIsbnAsync(string isbn);
    
    /// <summary>
    /// Get books by author
    /// </summary>
    Task<List<Book>> GetBooksByAuthorAsync(string author);
}
