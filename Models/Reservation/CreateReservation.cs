using System.ComponentModel.DataAnnotations;

namespace BistroBookMVC.Models.Reservation
{
    public class CreateReservation
    {
        public int Id { get; set; }

        // Guest details
        [Required]
        [Range(1, 100, ErrorMessage = "Guest count must be between 1 and 100.")]
        public int GuestCount { get; set; }

        // Reservation details
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }


        public int CustomerId { get; set; }
        public int TableId { get; set; }
    }
}

