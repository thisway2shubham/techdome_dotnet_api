using Loan_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Loan_App.Context
{
    public class LoanContext: DbContext
    {
        public LoanContext(DbContextOptions<LoanContext> options) : base(options)
        {
            
        }

        public DbSet<loan> Loans { get; set; }
        public DbSet<payment> Payments { get; set; }
        public DbSet<user> Users { get; set; }
    }
}
