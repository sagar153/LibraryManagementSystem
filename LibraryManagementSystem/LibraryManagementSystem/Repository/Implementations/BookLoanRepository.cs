namespace LibraryManagementSystem.Repository;

using Data;
using Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Book loan repository implementation using Entity Framework Core
/// </summary>
public class BookLoanRepository : IBookLoanRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookLoanRepository> _logger;

    public BookLoanRepository(ApplicationDbContext context, ILogger<BookLoanRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BookLoan>> GetAllLoansAsync()
    {
        try
        {
            return await _context.BookLoans
                .AsNoTracking()
                .OrderByDescending(bl => bl.CheckoutDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all loans: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<BookLoan?> GetLoanByIdAsync(int id)
    {
        try
        {
            return await _context.BookLoans
                .AsNoTracking()
                .FirstOrDefaultAsync(bl => bl.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting loan by ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<BookLoan>> GetLoansByMemberIdAsync(int memberId)
    {
        try
        {
            return await _context.BookLoans
                .AsNoTracking()
                .Where(bl => bl.MemberId == memberId)
                .OrderByDescending(bl => bl.CheckoutDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting loans for member {memberId}: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<List<BookLoan>> GetActiveLoansByMemberAsync(int memberId)
    {
        try
        {
            return await _context.BookLoans
                .AsNoTracking()
                .Where(bl => bl.MemberId == memberId && bl.Status == "Active")
                .OrderBy(bl => bl.DueDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting active loans for member {memberId}: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<List<BookLoan>> GetOverdueLoansAsync()
    {
        try
        {
            return await _context.BookLoans
                .AsNoTracking()
                .Where(bl => bl.Status == "Active" && bl.DueDate < DateTime.UtcNow)
                .OrderBy(bl => bl.DueDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting overdue loans: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<BookLoan> CreateLoanAsync(BookLoan loan)
    {
        try
        {
            loan.CheckoutDate = DateTime.UtcNow;
            loan.Status = "Active";
            loan.CreatedAt = DateTime.UtcNow;
            
            _context.BookLoans.Add(loan);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Loan created: Book {loan.BookId} to Member {loan.MemberId}");
            return loan;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating loan: {ex.Message}");
            throw;
        }
    }

    public async Task<BookLoan?> ReturnBookAsync(int loanId)
    {
        try
        {
            var loan = await _context.BookLoans.FirstOrDefaultAsync(bl => bl.Id == loanId);
            if (loan == null)
            {
                _logger.LogWarning($"Loan with ID {loanId} not found");
                return null;
            }

            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = "Returned";
            loan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Book returned: Loan {loanId}");
            return loan;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error returning book for loan {loanId}: {ex.Message}");
            return null;
        }
    }

    public async Task<BookLoan?> RenewLoanAsync(int loanId)
    {
        try
        {
            var loan = await _context.BookLoans.FirstOrDefaultAsync(bl => bl.Id == loanId);
            if (loan == null || loan.Status != "Active")
            {
                _logger.LogWarning($"Loan {loanId} cannot be renewed");
                return null;
            }

            loan.DueDate = loan.DueDate.AddDays(14);
            loan.RenewalCount = (loan.RenewalCount ?? 0) + 1;
            loan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Loan renewed: {loanId}, new due date: {loan.DueDate}");
            return loan;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error renewing loan {loanId}: {ex.Message}");
            return null;
        }
    }

    public async Task CalculateLateFeesAsync()
    {
        try
        {
            var overdueLoans = await _context.BookLoans
                .Where(bl => bl.Status == "Active" && bl.DueDate < DateTime.UtcNow)
                .ToListAsync();

            decimal feePerDay = 1.00m;

            foreach (var loan in overdueLoans)
            {
                var daysOverdue = (int)(DateTime.UtcNow - loan.DueDate).TotalDays;
                if (daysOverdue > 0)
                {
                    loan.LateFee = (loan.LateFee ?? 0) + (daysOverdue * feePerDay);
                    loan.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Late fees calculated for {overdueLoans.Count} loans");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calculating late fees: {ex.Message}");
        }
    }
}

/// <summary>
/// Book reservation repository implementation using Entity Framework Core
/// </summary>
public class BookReservationRepository : IBookReservationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookReservationRepository> _logger;

    public BookReservationRepository(ApplicationDbContext context, ILogger<BookReservationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BookReservation>> GetAllReservationsAsync()
    {
        try
        {
            return await _context.BookReservations
                .AsNoTracking()
                .OrderByDescending(br => br.ReservationDate)
                .ToListAsync();
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
            return await _context.BookReservations
                .AsNoTracking()
                .FirstOrDefaultAsync(br => br.Id == id);
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
            return await _context.BookReservations
                .AsNoTracking()
                .Where(br => br.MemberId == memberId)
                .OrderByDescending(br => br.ReservationDate)
                .ToListAsync();
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
            return await _context.BookReservations
                .AsNoTracking()
                .Where(br => br.BookId == bookId)
                .OrderBy(br => br.QueuePosition)
                .ToListAsync();
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
            return await _context.BookReservations
                .AsNoTracking()
                .Where(br => br.Status == "Pending")
                .OrderBy(br => br.QueuePosition)
                .ToListAsync();
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
            reservation.ReservationDate = DateTime.UtcNow;
            reservation.ExpiryDate = DateTime.UtcNow.AddDays(14);
            reservation.Status = "Pending";
            reservation.CreatedAt = DateTime.UtcNow;

            // Set queue position
            var maxQueue = await _context.BookReservations
                .Where(br => br.BookId == reservation.BookId)
                .MaxAsync(br => (int?)br.QueuePosition) ?? 0;
            reservation.QueuePosition = maxQueue + 1;

            _context.BookReservations.Add(reservation);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Reservation created: Book {reservation.BookId} for Member {reservation.MemberId}");
            return reservation;
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
            var reservation = await _context.BookReservations.FirstOrDefaultAsync(br => br.Id == id);
            if (reservation == null)
            {
                _logger.LogWarning($"Reservation with ID {id} not found");
                return null;
            }

            reservation.Status = status;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Reservation status updated: {id} to {status}");
            return reservation;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating reservation status {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        try
        {
            var reservation = await _context.BookReservations.FirstOrDefaultAsync(br => br.Id == id);
            if (reservation == null)
            {
                _logger.LogWarning($"Reservation with ID {id} not found");
                return false;
            }

            reservation.Status = "Cancelled";
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Reservation cancelled: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cancelling reservation {id}: {ex.Message}");
            return false;
        }
    }

    public async Task ProcessExpiredReservationsAsync()
    {
        try
        {
            var expiredReservations = await _context.BookReservations
                .Where(br => br.Status == "Pending" && br.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var reservation in expiredReservations)
            {
                reservation.Status = "Expired";
                reservation.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Processed {expiredReservations.Count} expired reservations");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing expired reservations: {ex.Message}");
        }
    }
}
