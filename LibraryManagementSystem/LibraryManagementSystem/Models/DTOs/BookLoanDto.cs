namespace LibraryManagementSystem.Models.DTOs;

/// <summary>
/// DTO for creating a book loan
/// </summary>
public class CreateBookLoanDto
{
    public int BookId { get; set; }
    
    public int MemberId { get; set; }
    
    public DateTime DueDate { get; set; }
}

/// <summary>
/// DTO for book loan response
/// </summary>
public class BookLoanDto
{
    public int Id { get; set; }
    
    public int BookId { get; set; }
    
    public int MemberId { get; set; }
    
    public DateTime CheckoutDate { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public DateTime? ReturnDate { get; set; }
    
    public string Status { get; set; } = string.Empty;
    
    public int? RenewalCount { get; set; }
    
    public decimal? LateFee { get; set; }
}
