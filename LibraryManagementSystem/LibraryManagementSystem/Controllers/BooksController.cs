namespace LibraryManagementSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Services.Interfaces;

/// <summary>
/// API controller for book management operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Get all books
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "User,Librarian,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
    {
        _logger.LogInformation("Fetching all books");
        var books = await _bookService.GetAllBooksAsync();
        var bookDtos = books.Select(MapToDto).ToList();
        return Ok(bookDtos);
    }

    /// <summary>
    /// Get book by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Librarian,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBookById(int id)
    {
        _logger.LogInformation($"Fetching book with ID: {id}");
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            _logger.LogWarning($"Book with ID {id} not found");
            return NotFound();
        }
        return Ok(MapToDto(book));
    }

    /// <summary>
    /// Search books by category
    /// </summary>
    [HttpGet("category/{category}")]
    [Authorize(Roles = "User,Librarian,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(string category)
    {
        _logger.LogInformation($"Fetching books in category: {category}");
        var books = await _bookService.GetBooksByCategoryAsync(category);
        var bookDtos = books.Select(MapToDto).ToList();
        return Ok(bookDtos);
    }

    /// <summary>
    /// Search books by title, author, or ISBN
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = "User,Librarian,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        _logger.LogInformation($"Searching books with term: {searchTerm}");
        var books = await _bookService.SearchBooksAsync(searchTerm);
        var bookDtos = books.Select(MapToDto).ToList();
        return Ok(bookDtos);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateUpdateBookDto createBookDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Creating new book: {createBookDto.Title}");
        
        var book = new Book
        {
            Title = createBookDto.Title,
            Author = createBookDto.Author,
            Isbn = createBookDto.Isbn,
            Publisher = createBookDto.Publisher,
            PublishDate = createBookDto.PublishDate,
            Category = createBookDto.Category,
            TotalCopies = createBookDto.TotalCopies,
            AvailableCopies = createBookDto.AvailableCopies,
            Description = createBookDto.Description
        };

        var createdBook = await _bookService.CreateBookAsync(book);
        return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, MapToDto(createdBook));
    }

    /// <summary>
    /// Update an existing book
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] CreateUpdateBookDto updateBookDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Updating book with ID: {id}");
        
        var book = new Book
        {
            Title = updateBookDto.Title,
            Author = updateBookDto.Author,
            Isbn = updateBookDto.Isbn,
            Publisher = updateBookDto.Publisher,
            PublishDate = updateBookDto.PublishDate,
            Category = updateBookDto.Category,
            TotalCopies = updateBookDto.TotalCopies,
            AvailableCopies = updateBookDto.AvailableCopies,
            Description = updateBookDto.Description
        };

        var updatedBook = await _bookService.UpdateBookAsync(id, book);
        if (updatedBook == null)
        {
            _logger.LogWarning($"Book with ID {id} not found for update");
            return NotFound();
        }
        return Ok(MapToDto(updatedBook));
    }

    /// <summary>
    /// Delete a book (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        _logger.LogInformation($"Deleting book with ID: {id}");
        var result = await _bookService.DeleteBookAsync(id);
        if (!result)
        {
            _logger.LogWarning($"Book with ID {id} not found for deletion");
            return NotFound();
        }
        return NoContent();
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Isbn = book.Isbn,
            Publisher = book.Publisher,
            PublishDate = book.PublishDate,
            Category = book.Category,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            Description = book.Description,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            IsActive = book.IsActive
        };
    }
}
