namespace LibraryManagementSystem.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents the Books table in the database
/// </summary>
[Table("Books")]
public class Book
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  [StringLength(500)]
  public string Title { get; set; } = string.Empty;

  [Required]
  [StringLength(256)]
  public string Author { get; set; } = string.Empty;

  [Required]
  [StringLength(20)]
  public string Isbn { get; set; } = string.Empty;

  [StringLength(256)]
  public string Publisher { get; set; } = string.Empty;

  public DateTime PublishDate { get; set; }

  [StringLength(100)]
  public string Category { get; set; } = string.Empty;

  public int TotalCopies { get; set; }

  public int AvailableCopies { get; set; }

  public string Description { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }

  public bool IsActive { get; set; } = true;
}
