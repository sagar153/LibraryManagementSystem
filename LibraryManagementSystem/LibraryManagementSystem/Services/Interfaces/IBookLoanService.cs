namespace LibraryManagementSystem.Services.Interfaces;

using Models;

/// <summary>
/// Interface for book loan service operations
/// </summary>
public interface IBookLoanService
{
    Task<List<BookLoan>> GetAllLoansAsync();
    
    Task<BookLoan?> GetLoanByIdAsync(int id);
    
    Task<List<BookLoan>> GetLoansByMemberIdAsync(int memberId);
    
    Task<List<BookLoan>> GetOverdueLoansAsync();
    
    Task<List<BookLoan>> GetActiveLoansByMemberAsync(int memberId);
    
    Task<BookLoan> CreateLoanAsync(BookLoan loan);
    
    Task<BookLoan?> ReturnBookAsync(int loanId);
    
    Task<BookLoan?> RenewLoanAsync(int loanId);
    
    Task<bool> CalculateLateFeesAsync();
}
