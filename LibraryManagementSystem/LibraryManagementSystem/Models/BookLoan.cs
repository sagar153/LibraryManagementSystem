namespace LibraryManagementSystem.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents the BookLoans table in the database
/// </summary>
[Table("BookLoans")]
public class BookLoan
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  public int BookId { get; set; }

  public int MemberId { get; set; }

  public DateTime CheckoutDate { get; set; } = DateTime.UtcNow;

  public DateTime DueDate { get; set; }

  public DateTime? ReturnDate { get; set; }

  [StringLength(50)]
  public string Status { get; set; } = "Active";

  public int? RenewalCount { get; set; } = 0;

  [Column(TypeName = "decimal(18,2)")]
  public decimal? LateFee { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }
}
