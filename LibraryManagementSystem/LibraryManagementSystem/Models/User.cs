namespace LibraryManagementSystem.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents the Users table in the database
/// </summary>
[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string PasswordSalt { get; set; } = string.Empty;

    [StringLength(256)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    [InverseProperty("User")]
    public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
}
