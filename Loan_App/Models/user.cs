using System.ComponentModel.DataAnnotations;

namespace Loan_App.Models
{
    public class user
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
    }
}
