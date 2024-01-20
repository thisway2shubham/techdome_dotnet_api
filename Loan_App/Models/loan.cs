using System.ComponentModel.DataAnnotations;

namespace Loan_App.Models
{
    public class loan
    {
        [Key]
        public int Id { get; set; }
        public int userId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int Term { get; set; }
        public decimal amountReceived { get; set; }

        [EnumDataType(typeof(LoanState))]
        public LoanState State { get; set; } = LoanState.PENDING;

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public ICollection<payment> Payments { get; set; }

        //public static implicit operator Loan(loan v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
