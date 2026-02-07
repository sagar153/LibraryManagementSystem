namespace LibraryManagementSystem.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents the Members table in the database
/// </summary>
[Table("Members")]
public class Member
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  [StringLength(256)]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  [StringLength(256)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [StringLength(256)]
  public string Email { get; set; } = string.Empty;

  [StringLength(20)]
  public string PhoneNumber { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string MembershipId { get; set; } = string.Empty;

  public DateTime JoinDate { get; set; } = DateTime.UtcNow;

  public DateTime? MembershipExpiryDate { get; set; }

  public string Address { get; set; } = string.Empty;

  public string City { get; set; } = string.Empty;

  public string PostalCode { get; set; } = string.Empty;

  public bool IsActive { get; set; } = true;

  public DateTime CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
}
