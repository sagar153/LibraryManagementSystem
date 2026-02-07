namespace LibraryManagementSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Services.Interfaces;

/// <summary>
/// API controller for book reservation operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BookReservationsController : ControllerBase
{
    private readonly IBookReservationService _reservationService;
    private readonly ILogger<BookReservationsController> _logger;

    public BookReservationsController(
        IBookReservationService reservationService,
        ILogger<BookReservationsController> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all reservations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookReservationDto>>> GetAllReservations()
    {
        _logger.LogInformation("Fetching all reservations");
        var reservations = await _reservationService.GetAllReservationsAsync();
        var reservationDtos = reservations.Select(MapToDto).ToList();
        return Ok(reservationDtos);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookReservationDto>> GetReservationById(int id)
    {
        _logger.LogInformation($"Fetching reservation with ID: {id}");
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null)
        {
            _logger.LogWarning($"Reservation with ID {id} not found");
            return NotFound();
        }
        return Ok(MapToDto(reservation));
    }

    /// <summary>
    /// Get all reservations for a member
    /// </summary>
    [HttpGet("member/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookReservationDto>>> GetReservationsByMemberId(int memberId)
    {
        _logger.LogInformation($"Fetching reservations for member ID: {memberId}");
        var reservations = await _reservationService.GetReservationsByMemberIdAsync(memberId);
        var reservationDtos = reservations.Select(MapToDto).ToList();
        return Ok(reservationDtos);
    }

    /// <summary>
    /// Get all reservations for a book
    /// </summary>
    [HttpGet("book/{bookId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookReservationDto>>> GetReservationsByBookId(int bookId)
    {
        _logger.LogInformation($"Fetching reservations for book ID: {bookId}");
        var reservations = await _reservationService.GetReservationsByBookIdAsync(bookId);
        var reservationDtos = reservations.Select(MapToDto).ToList();
        return Ok(reservationDtos);
    }

    /// <summary>
    /// Get all pending reservations
    /// </summary>
    [HttpGet("status/pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookReservationDto>>> GetPendingReservations()
    {
        _logger.LogInformation("Fetching pending reservations");
        var reservations = await _reservationService.GetPendingReservationsAsync();
        var reservationDtos = reservations.Select(MapToDto).ToList();
        return Ok(reservationDtos);
    }

    /// <summary>
    /// Create a new book reservation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookReservationDto>> CreateReservation([FromBody] CreateBookReservationDto createReservationDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Creating new reservation for member {createReservationDto.MemberId} for book {createReservationDto.BookId}");

        var reservation = new BookReservation
        {
            BookId = createReservationDto.BookId,
            MemberId = createReservationDto.MemberId,
            Status = "Pending"
        };

        var createdReservation = await _reservationService.CreateReservationAsync(reservation);
        return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.Id }, MapToDto(createdReservation));
    }

    /// <summary>
    /// Update reservation status
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookReservationDto>> UpdateReservationStatus(int id, [FromBody] dynamic statusUpdate)
    {
        _logger.LogInformation($"Updating status for reservation ID: {id}");
        
        string? status = statusUpdate?.status as string;
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status is required");

        var updatedReservation = await _reservationService.UpdateReservationStatusAsync(id, status);
        if (updatedReservation == null)
        {
            _logger.LogWarning($"Reservation with ID {id} not found");
            return NotFound();
        }
        return Ok(MapToDto(updatedReservation));
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelReservation(int id)
    {
        _logger.LogInformation($"Cancelling reservation ID: {id}");
        var result = await _reservationService.CancelReservationAsync(id);
        if (!result)
        {
            _logger.LogWarning($"Reservation with ID {id} not found");
            return NotFound();
        }
        return Ok(new { message = "Reservation cancelled successfully" });
    }

    /// <summary>
    /// Process expired reservations
    /// </summary>
    [HttpPost("process/expired")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessExpiredReservations()
    {
        _logger.LogInformation("Processing expired reservations");
        await _reservationService.ProcessExpiredReservationsAsync();
        return Ok(new { message = "Expired reservations processed successfully" });
    }

    private static BookReservationDto MapToDto(BookReservation reservation)
    {
        return new BookReservationDto
        {
            Id = reservation.Id,
            BookId = reservation.BookId,
            MemberId = reservation.MemberId,
            ReservationDate = reservation.ReservationDate,
            ExpiryDate = reservation.ExpiryDate,
            Status = reservation.Status,
            QueuePosition = reservation.QueuePosition
        };
    }
}
