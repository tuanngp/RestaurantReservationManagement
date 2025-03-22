using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationManagement.Models
{
    public class BookTableViewModel : IValidatableObject
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Seats { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian đặt bàn")]
        public DateTime ReservationDateTime { get; set; }

        // Validation tùy chỉnh cho thời gian đặt bàn
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReservationDateTime < DateTime.Now)
            {
                yield return new ValidationResult(
                    "Thời gian đặt bàn không được ở quá khứ",
                    new[] { nameof(ReservationDateTime) }
                );
            }

            var maxDate = DateTime.Now.AddDays(30);
            if (ReservationDateTime > maxDate)
            {
                yield return new ValidationResult(
                    "Chỉ được đặt bàn trong vòng 30 ngày",
                    new[] { nameof(ReservationDateTime) }
                );
            }

            var hour = ReservationDateTime.Hour;
            if (hour < 8 || hour >= 22)
            {
                yield return new ValidationResult(
                    "Nhà hàng chỉ hoạt động từ 8:00 đến 22:00",
                    new[] { nameof(ReservationDateTime) }
                );
            }
        }
    }
}
