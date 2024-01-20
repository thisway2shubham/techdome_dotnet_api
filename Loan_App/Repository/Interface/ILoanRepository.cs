using Loan_App.Models;


namespace Loan_App.Repository.Interface
{
    public interface ILoanRepository 
    {
        Task<IEnumerable<loan>> GetByUserId(string userId);
        Task Create(loan loan);
        Task<loan> GetById(string loanId);
        Task Update(loan loan);
        Task<bool> Delete(string loanId);
        Task<IEnumerable<loan>> GetAllLoans();

    }
}
