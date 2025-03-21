using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBusiness.Models;

public partial class Table
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Table number is required.")]
    public int TableNumber { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seats must be at least 1.")]
    public int Seats { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression(
        "^(Available|Reserved|Occupied)$",
        ErrorMessage = "Status must be 'Available', 'Reserved', or 'Occupied'."
    )]
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
