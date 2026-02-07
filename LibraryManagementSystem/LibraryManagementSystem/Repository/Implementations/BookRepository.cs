namespace LibraryManagementSystem.Repository;

using Data;
using Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Book repository implementation using Entity Framework Core
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookRepository> _logger;

    public BookRepository(ApplicationDbContext context, ILogger<BookRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all active books
    /// </summary>
    public async Task<List<Book>> GetAllBooksAsync()
    {
        try
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all books: {ex.Message}");
            return new List<Book>();
        }
    }

    /// <summary>
    /// Get book by ID
    /// </summary>
    public async Task<Book?> GetBookByIdAsync(int id)
    {
        try
        {
            return await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting book by ID {id}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get books by category
    /// </summary>
    public async Task<List<Book>> GetBooksByCategoryAsync(string category)
    {
        try
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => b.IsActive && b.Category != null && b.Category.ToLower() == category.ToLower())
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting books by category {category}: {ex.Message}");
            return new List<Book>();
        }
    }

    /// <summary>
    /// Search books by title, author, or ISBN
    /// </summary>
    public async Task<List<Book>> SearchBooksAsync(string searchTerm)
    {
        try
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Books
                .AsNoTracking()
                .Where(b => b.IsActive && (
                    b.Title.ToLower().Contains(lowerSearchTerm) ||
                    b.Author.ToLower().Contains(lowerSearchTerm) ||
                    b.Isbn.ToLower().Contains(lowerSearchTerm)
                ))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching books with term '{searchTerm}': {ex.Message}");
            return new List<Book>();
        }
    }

    /// <summary>
    /// Create new book
    /// </summary>
    public async Task<Book> CreateBookAsync(Book book)
    {
        try
        {
            book.CreatedAt = DateTime.UtcNow;
            book.IsActive = true;
            
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Book created: {book.Title} (ISBN: {book.Isbn})");
            return book;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating book: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Update existing book
    /// </summary>
    public async Task<Book?> UpdateBookAsync(int id, Book book)
    {
        try
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (existingBook == null)
            {
                _logger.LogWarning($"Book with ID {id} not found");
                return null;
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Isbn = book.Isbn;
            existingBook.Publisher = book.Publisher;
            existingBook.PublishDate = book.PublishDate;
            existingBook.Category = book.Category;
            existingBook.TotalCopies = book.TotalCopies;
            existingBook.AvailableCopies = book.AvailableCopies;
            existingBook.Description = book.Description;
            existingBook.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Book updated: {existingBook.Title} (ID: {id})");
            return existingBook;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating book {id}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete book (soft delete)
    /// </summary>
    public async Task<bool> DeleteBookAsync(int id)
    {
        try
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                _logger.LogWarning($"Book with ID {id} not found");
                return false;
            }

            book.IsActive = false;
            book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Book deleted (soft): {book.Title} (ID: {id})");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting book {id}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Update available copies
    /// </summary>
    public async Task<bool> UpdateAvailableCopiesAsync(int bookId, int quantity)
    {
        try
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                _logger.LogWarning($"Book with ID {bookId} not found");
                return false;
            }

            book.AvailableCopies = Math.Max(0, Math.Min(book.TotalCopies, book.AvailableCopies + quantity));
            book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Available copies updated for book {bookId}: {book.AvailableCopies}/{book.TotalCopies}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating available copies for book {bookId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if book exists by ISBN
    /// </summary>
    public async Task<bool> BookExistsByIsbnAsync(string isbn)
    {
        try
        {
            return await _context.Books.AnyAsync(b => b.Isbn == isbn && b.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking book existence by ISBN {isbn}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get books by author
    /// </summary>
    public async Task<List<Book>> GetBooksByAuthorAsync(string author)
    {
        try
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => b.IsActive && b.Author.ToLower() == author.ToLower())
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting books by author {author}: {ex.Message}");
            return new List<Book>();
        }
    }
}
