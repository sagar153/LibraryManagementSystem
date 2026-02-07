namespace LibraryManagementSystem.Services.Implementations;

using Models;
using Repository.Interfaces;
using Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementation of book loan service using database transactions
/// </summary>
public class BookLoanService : IBookLoanService
{
    private readonly IBookLoanRepository _loanRepository;
    private readonly ILogger<BookLoanService> _logger;
    private const decimal DailyLateFee = 0.5m;

    public BookLoanService(
        IBookLoanRepository loanRepository,
        ILogger<BookLoanService> logger)
    {
        _loanRepository = loanRepository;
        _logger = logger;
    }

    public async Task<List<BookLoan>> GetAllLoansAsync()
    {
        try
        {
            return await _loanRepository.GetAllLoansAsync();
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
            return await _loanRepository.GetLoanByIdAsync(id);
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
            return await _loanRepository.GetLoansByMemberIdAsync(memberId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting loans for member {memberId}: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<List<BookLoan>> GetOverdueLoansAsync()
    {
        try
        {
            return await _loanRepository.GetOverdueLoansAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting overdue loans: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<List<BookLoan>> GetActiveLoansByMemberAsync(int memberId)
    {
        try
        {
            return await _loanRepository.GetActiveLoansByMemberAsync(memberId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting active loans for member {memberId}: {ex.Message}");
            return new List<BookLoan>();
        }
    }

    public async Task<BookLoan> CreateLoanAsync(BookLoan loan)
    {
        try
        {
            var createdLoan = await _loanRepository.CreateLoanAsync(loan);
            _logger.LogInformation($"Loan created: Book {loan.BookId} loaned to Member {loan.MemberId}");
            return createdLoan;
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
            var returnedLoan = await _loanRepository.ReturnBookAsync(loanId);
            if (returnedLoan != null)
            {
                _logger.LogInformation($"Book returned: Loan {loanId}");
            }
            return returnedLoan;
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
            var renewedLoan = await _loanRepository.RenewLoanAsync(loanId);
            if (renewedLoan != null)
            {
                _logger.LogInformation($"Loan renewed: {loanId}, new due date: {renewedLoan.DueDate}");
            }
            return renewedLoan;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error renewing loan {loanId}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CalculateLateFeesAsync()
    {
        try
        {
            await _loanRepository.CalculateLateFeesAsync();
            _logger.LogInformation("Late fees calculated for overdue loans");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calculating late fees: {ex.Message}");
            return false;
        }
    }
}
