namespace LibraryManagementSystem.Models.DTOs;

/// <summary>
/// DTO for creating or updating a book
/// </summary>
public class CreateUpdateBookDto
{
    public string Title { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
    
    public string Isbn { get; set; } = string.Empty;
    
    public string Publisher { get; set; } = string.Empty;
    
    public DateTime PublishDate { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public int TotalCopies { get; set; }
    
    public int AvailableCopies { get; set; }
    
    public string Description { get; set; } = string.Empty;
}
