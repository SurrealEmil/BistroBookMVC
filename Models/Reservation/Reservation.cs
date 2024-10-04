using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BistroBookMVC.Models.Reservation
{
    public class Reservation
    {
        public int Id { get; set; }

        // Guest details
        [Required]
        [Range(1, 20, ErrorMessage = "Guest count must be between 1 and 20.")]
        public int GuestCount { get; set; }
        public string CustomerFullName { get; set; }
        public int TableNumber { get; set; }

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
