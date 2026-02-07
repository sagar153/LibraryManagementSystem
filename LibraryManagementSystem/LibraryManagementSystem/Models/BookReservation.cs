namespace LibraryManagementSystem.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents the BookReservations table in the database
/// </summary>
[Table("BookReservations")]
public class BookReservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int BookId { get; set; }

    public int MemberId { get; set; }

    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiryDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    public int QueuePosition { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
