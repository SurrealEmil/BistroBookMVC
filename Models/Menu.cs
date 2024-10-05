using System.ComponentModel.DataAnnotations;

namespace BistroBookMVC.Models
{
    public class Menu
    {
        public int Id { get; set; }

        // Dish details
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Dish name must be between 1 and 50 characters.")]
        public string DishName { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 200 characters.")]
        public string Description { get; set; }

        // Price of the dish
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public int Price { get; set; }

        // Favorite status
        public bool IsFavorite { get; set; }

        // Availability status
        public bool IsAvailable { get; set; }
    }
}
