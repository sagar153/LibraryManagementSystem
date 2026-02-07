namespace LibraryManagementSystem.Repository.Interfaces;

using Models;

/// <summary>
/// Interface for book loan repository operations
/// </summary>
public interface IBookLoanRepository
{
    /// <summary>
    /// Get all loans
    /// </summary>
    Task<List<BookLoan>> GetAllLoansAsync();
    
    /// <summary>
    /// Get loan by ID
    /// </summary>
    Task<BookLoan?> GetLoanByIdAsync(int id);
    
    /// <summary>
    /// Get loans by member ID
    /// </summary>
    Task<List<BookLoan>> GetLoansByMemberIdAsync(int memberId);
    
    /// <summary>
    /// Get active loans for member
    /// </summary>
    Task<List<BookLoan>> GetActiveLoansByMemberAsync(int memberId);
    
    /// <summary>
    /// Get overdue loans
    /// </summary>
    Task<List<BookLoan>> GetOverdueLoansAsync();
    
    /// <summary>
    /// Create loan
    /// </summary>
    Task<BookLoan> CreateLoanAsync(BookLoan loan);
    
    /// <summary>
    /// Return book
    /// </summary>
    Task<BookLoan?> ReturnBookAsync(int loanId);
    
    /// <summary>
    /// Renew loan
    /// </summary>
    Task<BookLoan?> RenewLoanAsync(int loanId);
    
    /// <summary>
    /// Calculate late fees for overdue loans
    /// </summary>
    Task CalculateLateFeesAsync();
}

/// <summary>
/// Interface for book reservation repository operations
/// </summary>
public interface IBookReservationRepository
{
    /// <summary>
    /// Get all reservations
    /// </summary>
    Task<List<BookReservation>> GetAllReservationsAsync();
    
    /// <summary>
    /// Get reservation by ID
    /// </summary>
    Task<BookReservation?> GetReservationByIdAsync(int id);
    
    /// <summary>
    /// Get reservations by member ID
    /// </summary>
    Task<List<BookReservation>> GetReservationsByMemberIdAsync(int memberId);
    
    /// <summary>
    /// Get reservations by book ID
    /// </summary>
    Task<List<BookReservation>> GetReservationsByBookIdAsync(int bookId);
    
    /// <summary>
    /// Get pending reservations
    /// </summary>
    Task<List<BookReservation>> GetPendingReservationsAsync();
    
    /// <summary>
    /// Create reservation
    /// </summary>
    Task<BookReservation> CreateReservationAsync(BookReservation reservation);
    
    /// <summary>
    /// Update reservation status
    /// </summary>
    Task<BookReservation?> UpdateReservationStatusAsync(int id, string status);
    
    /// <summary>
    /// Cancel reservation
    /// </summary>
    Task<bool> CancelReservationAsync(int id);
    
    /// <summary>
    /// Process expired reservations
    /// </summary>
    Task ProcessExpiredReservationsAsync();
}
