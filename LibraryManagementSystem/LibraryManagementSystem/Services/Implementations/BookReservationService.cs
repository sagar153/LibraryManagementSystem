namespace LibraryManagementSystem.Services.Implementations;

using Models;
using Repository.Interfaces;
using Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementation of book reservation service using database transactions
/// </summary>
public class BookReservationService : IBookReservationService
{
    private readonly IBookReservationRepository _reservationRepository;
    private readonly ILogger<BookReservationService> _logger;
    private const int ReservationExpiryDays = 7;

    public BookReservationService(
        IBookReservationRepository reservationRepository,
        ILogger<BookReservationService> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public async Task<List<BookReservation>> GetAllReservationsAsync()
    {
        try
        {
            return await _reservationRepository.GetAllReservationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all reservations: {ex.Message}");
            return new List<BookReservation>();
        }
    }

    public async Task<BookReservation?> GetReservationByIdAsync(int id)
    {
        try
        {
            return await _reservationRepository.GetReservationByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting reservation by ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<BookReservation>> GetReservationsByMemberIdAsync(int memberId)
    {
        try
        {
            var reservations = await _reservationRepository.GetReservationsByMemberIdAsync(memberId);
            return reservations.Where(r => r.Status != "Cancelled").ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting reservations for member {memberId}: {ex.Message}");
            return new List<BookReservation>();
        }
    }

    public async Task<List<BookReservation>> GetReservationsByBookIdAsync(int bookId)
    {
        try
        {
            return await _reservationRepository.GetReservationsByBookIdAsync(bookId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting reservations for book {bookId}: {ex.Message}");
            return new List<BookReservation>();
        }
    }

    public async Task<List<BookReservation>> GetPendingReservationsAsync()
    {
        try
        {
            return await _reservationRepository.GetPendingReservationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting pending reservations: {ex.Message}");
            return new List<BookReservation>();
        }
    }

    public async Task<BookReservation> CreateReservationAsync(BookReservation reservation)
    {
        try
        {
            reservation.Status = "Pending";
            reservation.ExpiryDate = DateTime.UtcNow.AddDays(ReservationExpiryDays);
            
            var createdReservation = await _reservationRepository.CreateReservationAsync(reservation);
            _logger.LogInformation($"Reservation created for book {reservation.BookId} by member {reservation.MemberId}");
            
            return createdReservation;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating reservation: {ex.Message}");
            throw;
        }
    }

    public async Task<BookReservation?> UpdateReservationStatusAsync(int id, string status)
    {
        try
        {
            var updatedReservation = await _reservationRepository.UpdateReservationStatusAsync(id, status);
            if (updatedReservation != null)
            {
                _logger.LogInformation($"Reservation {id} status updated to {status}");
            }
            return updatedReservation;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating reservation {id} status: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        try
        {
            var result = await _reservationRepository.CancelReservationAsync(id);
            if (result)
            {
                _logger.LogInformation($"Reservation {id} cancelled");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cancelling reservation {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessExpiredReservationsAsync()
    {
        try
        {
            await _reservationRepository.ProcessExpiredReservationsAsync();
            _logger.LogInformation("Expired reservations processed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing expired reservations: {ex.Message}");
            return false;
        }
    }
}
