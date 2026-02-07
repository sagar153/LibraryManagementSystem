namespace LibraryManagementSystem.Models.DTOs;

/// <summary>
/// DTO for creating a book reservation
/// </summary>
public class CreateBookReservationDto
{
    public int BookId { get; set; }
    
    public int MemberId { get; set; }
}

/// <summary>
/// DTO for book reservation response
/// </summary>
public class BookReservationDto
{
    public int Id { get; set; }
    
    public int BookId { get; set; }
    
    public int MemberId { get; set; }
    
    public DateTime ReservationDate { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public string Status { get; set; } = string.Empty;
    
    public int QueuePosition { get; set; }
}
