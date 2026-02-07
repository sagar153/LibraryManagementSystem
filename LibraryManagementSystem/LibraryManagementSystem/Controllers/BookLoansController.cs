namespace LibraryManagementSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Services.Interfaces;

/// <summary>
/// API controller for book loan operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BookLoansController : ControllerBase
{
    private readonly IBookLoanService _bookLoanService;
    private readonly IBookService _bookService;
    private readonly ILogger<BookLoansController> _logger;

    public BookLoansController(
        IBookLoanService bookLoanService,
        IBookService bookService,
        ILogger<BookLoansController> logger)
    {
        _bookLoanService = bookLoanService;
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Get all book loans
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookLoanDto>>> GetAllLoans()
    {
        _logger.LogInformation("Fetching all book loans");
        var loans = await _bookLoanService.GetAllLoansAsync();
        var loanDtos = loans.Select(MapToDto).ToList();
        return Ok(loanDtos);
    }

    /// <summary>
    /// Get loan by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookLoanDto>> GetLoanById(int id)
    {
        _logger.LogInformation($"Fetching loan with ID: {id}");
        var loan = await _bookLoanService.GetLoanByIdAsync(id);
        if (loan == null)
        {
            _logger.LogWarning($"Loan with ID {id} not found");
            return NotFound();
        }
        return Ok(MapToDto(loan));
    }

    /// <summary>
    /// Get all loans for a member
    /// </summary>
    [HttpGet("member/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookLoanDto>>> GetLoansByMemberId(int memberId)
    {
        _logger.LogInformation($"Fetching loans for member ID: {memberId}");
        var loans = await _bookLoanService.GetLoansByMemberIdAsync(memberId);
        var loanDtos = loans.Select(MapToDto).ToList();
        return Ok(loanDtos);
    }

    /// <summary>
    /// Get all overdue loans
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookLoanDto>>> GetOverdueLoans()
    {
        _logger.LogInformation("Fetching overdue loans");
        var loans = await _bookLoanService.GetOverdueLoansAsync();
        var loanDtos = loans.Select(MapToDto).ToList();
        return Ok(loanDtos);
    }

    /// <summary>
    /// Get active loans for a member
    /// </summary>
    [HttpGet("member/{memberId}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookLoanDto>>> GetActiveLoansByMember(int memberId)
    {
        _logger.LogInformation($"Fetching active loans for member ID: {memberId}");
        var loans = await _bookLoanService.GetActiveLoansByMemberAsync(memberId);
        var loanDtos = loans.Select(MapToDto).ToList();
        return Ok(loanDtos);
    }

    /// <summary>
    /// Create a new book loan
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookLoanDto>> CreateLoan([FromBody] CreateBookLoanDto createLoanDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Creating new loan for member {createLoanDto.MemberId} for book {createLoanDto.BookId}");

        // Verify book exists and has available copies
        var book = await _bookService.GetBookByIdAsync(createLoanDto.BookId);
        if (book == null || book.AvailableCopies <= 0)
        {
            _logger.LogWarning($"Book {createLoanDto.BookId} not available for loan");
            return BadRequest("Book not available for loan");
        }

        var loan = new BookLoan
        {
            BookId = createLoanDto.BookId,
            MemberId = createLoanDto.MemberId,
            DueDate = createLoanDto.DueDate,
            Status = "Active"
        };

        var createdLoan = await _bookLoanService.CreateLoanAsync(loan);
        
        // Update available copies
        await _bookService.UpdateAvailableCopiesAsync(createLoanDto.BookId, -1);

        return CreatedAtAction(nameof(GetLoanById), new { id = createdLoan.Id }, MapToDto(createdLoan));
    }

    /// <summary>
    /// Return a borrowed book
    /// </summary>
    [HttpPost("{id}/return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookLoanDto>> ReturnBook(int id)
    {
        _logger.LogInformation($"Processing return for loan ID: {id}");
        var loan = await _bookLoanService.GetLoanByIdAsync(id);
        if (loan == null)
        {
            _logger.LogWarning($"Loan {id} not found");
            return NotFound();
        }

        var returnedLoan = await _bookLoanService.ReturnBookAsync(id);
        if (returnedLoan == null)
            return NotFound();

        // Update available copies
        await _bookService.UpdateAvailableCopiesAsync(loan.BookId, 1);

        return Ok(MapToDto(returnedLoan));
    }

    /// <summary>
    /// Renew a book loan
    /// </summary>
    [HttpPost("{id}/renew")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookLoanDto>> RenewLoan(int id)
    {
        _logger.LogInformation($"Renewing loan ID: {id}");
        var renewedLoan = await _bookLoanService.RenewLoanAsync(id);
        if (renewedLoan == null)
        {
            _logger.LogWarning($"Loan {id} cannot be renewed");
            return BadRequest("Loan cannot be renewed");
        }
        return Ok(MapToDto(renewedLoan));
    }

    /// <summary>
    /// Calculate late fees for overdue loans
    /// </summary>
    [HttpPost("process/late-fees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CalculateLateFees()
    {
        _logger.LogInformation("Calculating late fees for overdue loans");
        await _bookLoanService.CalculateLateFeesAsync();
        return Ok(new { message = "Late fees calculated successfully" });
    }

    private static BookLoanDto MapToDto(BookLoan loan)
    {
        return new BookLoanDto
        {
            Id = loan.Id,
            BookId = loan.BookId,
            MemberId = loan.MemberId,
            CheckoutDate = loan.CheckoutDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            RenewalCount = loan.RenewalCount,
            LateFee = loan.LateFee
        };
    }
}
