using System.ComponentModel.DataAnnotations;

namespace RestaurantBusiness.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? TableId { get; set; }

    public DateTime? ReservationDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression(
        "^(Pending|Confirmed|Cancelled)$",
        ErrorMessage = "Status must be 'Pending', 'Confirmed', or 'Cancelled'."
    )]
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Table? Table { get; set; }

    public virtual User? User { get; set; }
}
