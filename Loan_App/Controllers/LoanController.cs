using Loan_App.Models;
using Loan_App.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loan_App.Controllers
{
    [ApiController]
    [Route("/api[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanRepository _loanRepository; // Replace with your loan repository

        public LoanController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        [HttpPost("createLoan")]
        public async Task<IActionResult> CreateLoan([FromBody] loan loanRequest)
        {
            try
            {
                //var UserId = HttpContext.User.FindFirst("Id")?.Value; // Get user ID from claims or HttpContext
                //var uID = HttpContext.User.FindFirst("userId")?.Value;
                var UserId = loanRequest.userId;
                var amountRecieved = loanRequest.amountReceived;
                var loanTerm = loanRequest.Term;

                var weeklyRepayment = amountRecieved / loanTerm;
                var scheduledRepayments = new List<payment>();
                var currentDate = DateTime.Now;

                for (var i = 0; i < loanTerm; i++)
                {
                    var repaymentDate = currentDate.AddDays(7 * (i + 1));

                    scheduledRepayments.Add(new payment
                    {
                        Date = repaymentDate,
                        Amount = Math.Round(weeklyRepayment, 2),
                        Status = RepaymentStatus.PENDING 
                    });
                }

                var Loan = new loan
                {
                    userId = UserId,
                    amountReceived = amountRecieved,
                    Term = loanTerm,
                    Payments = scheduledRepayments
                };

                await _loanRepository.Create(Loan);

                return StatusCode(201, new { message = "Loan created successfully", Loan });
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return StatusCode(500, new { message = "Error creating loan" });
            }
        }

        [HttpGet("getLoansByUser")]
        public async Task<IActionResult> GetLoansByUser()
        {
            try
            {
                var userId = HttpContext.User.FindFirst("Id")?.Value; // Get user ID from claims or HttpContext

                var loans = await _loanRepository.GetByUserId(userId);

                return Ok(loans);
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return StatusCode(500, new { message = "Error fetching loans" });
            }
        }


        [HttpPost("approveLoan/{loanId}")]
        public async Task<IActionResult> ApproveLoan(string loanId)
        {
            try
            {
                var userRole = HttpContext.User.FindFirst("Role")?.Value; // Get user role from claims or HttpContext

                if (userRole != "Admin")
                {
                    return StatusCode(403, new { message = "Permission denied" });
                }

                var loan = await _loanRepository.GetById(loanId);

                if (loan == null)
                {
                    return NotFound(new { message = "Loan not found" });
                }

                loan.State = LoanState.APPROVED; 

                await _loanRepository.Update(loan);

                return Ok(new { message = "Loan approved successfully", loan });
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return StatusCode(500, new { message = "Error approving loan" });
            }
        }

        [HttpPost("addRepayment/{loanId}")]
        public async Task<IActionResult> AddRepayment(string loanId, payment repaymentRequest)
        {
            try
            {
                var loan = await _loanRepository.GetById(loanId);

                if (loan == null)
                {
                    return NotFound(new { message = "Loan not found" });
                }

                if (loan.State == LoanState.PAID)
                {
                    return BadRequest(new { message = "Loan Already Paid" });
                }

                var pendingRepayment = loan.Payments.FirstOrDefault(r => r.Status == RepaymentStatus.PENDING);

                if (pendingRepayment == null)
                {
                    return BadRequest(new { message = "No pending repayments for this loan" });
                }

                if (repaymentRequest.Amount < pendingRepayment.Amount)
                {
                    return BadRequest(new { message = "Invalid repayment amount" });
                }

                pendingRepayment.Status = RepaymentStatus.PAID;

                var remainingPendingRepayments = loan.Payments.Count(r => r.Status == RepaymentStatus.PENDING);

                if (remainingPendingRepayments == 0)
                {
                    loan.State = LoanState.PAID;
                }

                await _loanRepository.Update(loan);

                return Ok(new { message = "Repayment added successfully", loan });
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return StatusCode(500, new { message = "Error adding repayment" });
            }
        }

        [HttpGet("getAllLoans")]
        public async Task<IActionResult> GetAllLoans()
        {
            try
            {
                var loans = await _loanRepository.GetAllLoans();

                return Ok(loans);
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return StatusCode(500, new { message = "Error fetching all loans" });
            }
        }
    }
}
