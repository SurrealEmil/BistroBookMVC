using System.ComponentModel.DataAnnotations;

namespace BistroBookMVC.Models.Reservation
{
    public class CreateReservation
    {
        public int Id { get; set; }

        // Guest details
        [Required]
        [Range(1, 20, ErrorMessage = "Guest count must be between 1 and 20.")]
        public int GuestCount { get; set; }

        [Required]
        [Phone]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 20 characters.")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Email address can be a maximum of 100 characters.")]
        public string Email { get; set; }

        // Personal Details
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters.")]
        public string LastName { get; set; }

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
