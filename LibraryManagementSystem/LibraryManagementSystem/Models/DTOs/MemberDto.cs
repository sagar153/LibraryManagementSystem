namespace LibraryManagementSystem.Models.DTOs;

/// <summary>
/// DTO for member response
/// </summary>
public class MemberDto
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string MembershipId { get; set; } = string.Empty;
    
    public DateTime JoinDate { get; set; }
    
    public DateTime? MembershipExpiryDate { get; set; }
    
    public string Address { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string PostalCode { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
