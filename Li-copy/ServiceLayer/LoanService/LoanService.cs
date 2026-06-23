using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.Models.Fine;
using Li_copy.Models.Loans;

namespace Li_copy.ServiceLayer.LoanService
{
    public class LoanService : IloanService
    {
        private readonly IloanDLL _loanRepository;
        private readonly IfineDLL _fineDLL;
        public LoanService(IloanDLL loanRepository,IfineDLL ifineDLL)
        {
            _loanRepository = loanRepository;
            _fineDLL = ifineDLL;
        }
        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _loanRepository.GetAllLoansAsync();
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            return await _loanRepository.GetLoanByIdAsync(id);
        }

        public async Task<int> CreateLoanAsync(Loan loan)
        {
            return await _loanRepository.CreateLoanAsync(loan);
        }

        public async Task<bool> UpdateLoanAsync(Loan loan)
        {
            return await _loanRepository.UpdateLoanAsync(loan);
        }

        public async Task<bool> DeleteLoanAsync(int id)
        {
            return await _loanRepository.DeleteLoanAsync(id);
        }

        public async Task<bool> ReturnBookAsync(int loanId, DateTime returnDate)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null) return false;

            loan.DueDate = returnDate;
            loan.Status = "Returned";

            await _loanRepository.UpdateLoanAsync(loan);

            if (returnDate > loan.DueDate)
            {
                int extraDays = (returnDate.Date - loan.DueDate.Date).Days;
                if (extraDays > 0)
                {
                    decimal fineAmount = extraDays * 5.0m;

                    var existingFine = await _fineDLL.GetFineByLoanIdAsync(loanId);
                    if (existingFine == null)
                    {
                        var newFine = new Fine
                        {
                            LoanId = loanId,
                            Amount = fineAmount,
                            IsPaid = false,
                            PaymentStatus = "Pending"
                        };
                        await _fineDLL.CreateFineAsync(newFine);
                    }
                    else if (!existingFine.IsPaid)
                    {
                        existingFine.Amount = fineAmount;
                        await _fineDLL.UpdateFineAsync(existingFine);
                    }
                }
            }
            return true;
        }
    }
}
