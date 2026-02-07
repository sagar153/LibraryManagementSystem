namespace LibraryManagementSystem.Services.Implementations;

using Models;
using Interfaces;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Repository.Interfaces;

/// <summary>
/// Implementation of book service using database transactions
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BookService> _logger;

    public BookService(
        IBookRepository bookRepository,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        try
        {
            return await _bookRepository.GetAllBooksAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all books: {ex.Message}");
            return new List<Book>();
        }
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        try
        {
            return await _bookRepository.GetBookByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting book by ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Book>> GetBooksByCategoryAsync(string category)
    {
        try
        {
            return await _bookRepository.GetBooksByCategoryAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting books by category {category}: {ex.Message}");
            return new List<Book>();
        }
    }

    public async Task<List<Book>> SearchBooksAsync(string searchTerm)
    {
        try
        {
            return await _bookRepository.SearchBooksAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching books with term '{searchTerm}': {ex.Message}");
            return new List<Book>();
        }
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        try
        {
            var createdBook = await _bookRepository.CreateBookAsync(book);
            _logger.LogInformation($"Book created: {book.Title} (ISBN: {book.Isbn})");
            return createdBook;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating book: {ex.Message}");
            throw;
        }
    }

    public async Task<Book?> UpdateBookAsync(int id, Book book)
    {
        try
        {
            var updatedBook = await _bookRepository.UpdateBookAsync(id, book);
            if (updatedBook != null)
            {
                _logger.LogInformation($"Book updated: {book.Title} (ID: {id})");
            }
            return updatedBook;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating book {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        try
        {
            var result = await _bookRepository.DeleteBookAsync(id);
            if (result)
            {
                _logger.LogInformation($"Book deleted (soft): ID {id}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting book {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateAvailableCopiesAsync(int bookId, int quantity)
    {
        try
        {
            var result = await _bookRepository.UpdateAvailableCopiesAsync(bookId, quantity);
            if (result)
            {
                _logger.LogInformation($"Available copies updated for book {bookId}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating available copies for book {bookId}: {ex.Message}");
            return false;
        }
    }
}
