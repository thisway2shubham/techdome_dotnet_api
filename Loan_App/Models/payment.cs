using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loan_App.Models
{
    public class payment
    {
        [Key]
        public int Id { get; set; } 

        public int LoanId { get; set; } 

        [ForeignKey("LoanId")]
        public loan Loan { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [EnumDataType(typeof(RepaymentStatus))]
        public RepaymentStatus Status { get; set; } = RepaymentStatus.PENDING;

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 

    }
}
