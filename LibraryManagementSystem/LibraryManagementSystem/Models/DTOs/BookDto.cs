namespace LibraryManagementSystem.Models.DTOs;

/// <summary>
/// DTO for book response
/// </summary>
public class BookDto
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
    
    public string Isbn { get; set; } = string.Empty;
    
    public string Publisher { get; set; } = string.Empty;
    
    public DateTime PublishDate { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public int TotalCopies { get; set; }
    
    public int AvailableCopies { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsActive { get; set; }
}
