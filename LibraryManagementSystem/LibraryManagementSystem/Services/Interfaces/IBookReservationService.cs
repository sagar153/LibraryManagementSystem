namespace LibraryManagementSystem.Services.Interfaces;

using Models;

/// <summary>
/// Interface for book reservation service operations
/// </summary>
public interface IBookReservationService
{
    Task<List<BookReservation>> GetAllReservationsAsync();
    
    Task<BookReservation?> GetReservationByIdAsync(int id);
    
    Task<List<BookReservation>> GetReservationsByMemberIdAsync(int memberId);
    
    Task<List<BookReservation>> GetReservationsByBookIdAsync(int bookId);
    
    Task<List<BookReservation>> GetPendingReservationsAsync();
    
    Task<BookReservation> CreateReservationAsync(BookReservation reservation);
    
    Task<BookReservation?> UpdateReservationStatusAsync(int id, string status);
    
    Task<bool> CancelReservationAsync(int id);
    
    Task<bool> ProcessExpiredReservationsAsync();
}
