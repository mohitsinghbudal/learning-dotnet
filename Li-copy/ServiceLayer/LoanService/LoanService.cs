using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.Models.Book;
using Li_copy.Models.Fine;
using Li_copy.Models.Loans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Li_copy.ServiceLayer.LoanService
{
    public class LoanService : IloanService
    {
        private readonly IloanDLL _loanRepository;
        private readonly IfineDLL _fineDLL;
        private readonly IBooksDLL _bookDLL;

        public LoanService(IloanDLL loanRepository, IfineDLL ifineDLL, IBooksDLL booksDLL)
        {
            _loanRepository = loanRepository;
            _fineDLL = ifineDLL;
            _bookDLL = booksDLL;
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

        // Step 1: Student initiates the drop-off request
        public async Task<bool> ReturnBookAsync(int loanId)
        {
            // Fetch the existing loan details
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null || loan.Status == "Returned" || loan.Status == "Pending Return") return false;

            // Update the loan record status to alert teachers on frontend dashboard
            loan.ReturnDate = DateTime.Now;
            loan.Status = "Pending Return";

            return await _loanRepository.UpdateLoanAsync(loan);
        }

        // Step 2: Teacher verifies the return conditions, processes fines, and releases stock
        public async Task<int> VerifyReturnAsync(int loanId, int teacherId)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null || loan.Status != "Pending Return") return 102; // Fault/Error

            loan.Status = "Returned";
            loan.VerifiedByUserId = teacherId;
            loan.VerifiedAt = DateTime.Now;

            var inventoryReleased = await _loanRepository.UpdateAndReleaseInventoryAsync(loan);
            if (!inventoryReleased) return 102; // Fault/Error

            // Late return check
            if (loan.ReturnDate.HasValue && loan.DueDate.HasValue && loan.ReturnDate.Value.Date > loan.DueDate.Value.Date)
            {
                int extraDays = (loan.ReturnDate.Value.Date - loan.DueDate.Value.Date).Days;
                decimal fineAmount = extraDays * 5.0m;

                var existingFine = await _fineDLL.GetFineByLoanIdAsync(loanId);
                if (existingFine == null)
                {
                    await _fineDLL.CreateFineAsync(new Fine { LoanId = loanId, Amount = fineAmount, IsPaid = false, PaymentStatus = "Pending" });
                }
                else if (!existingFine.IsPaid)
                {
                    existingFine.Amount = fineAmount;
                    await _fineDLL.UpdateFineAsync(existingFine);
                }

                return 101; // Fine exists, redirect to payment
            }

            return 100; // Success, no fine
        }

        public async Task<IEnumerable<LoanHistoryDto>> GetPersonalHistoryAsync(int userId, int roleId)
        {
            // Restrict personal history access to Student (2) or Teacher (3)
            if (roleId != 2 && roleId != 3)
            {
                throw new UnauthorizedAccessException("Unauthorized role access requested.");
            }

            return await _loanRepository.GetPersonalBorrowHistoryAsync(userId);
        }

        public async Task<IEnumerable<LoanHistoryDto>> GetGlobalBorrowRecordsAsync(int roleId)
        {
            // Restrict global lists strictly to Teacher (3) or Admin (1) to match your Controller layer policies
            if (roleId != 3 && roleId != 1)
            {
                throw new UnauthorizedAccessException("Only Teachers or Higher Authorities can view all records.");
            }

            return await _loanRepository.GetAllBorrowedBooksAsync();
        }

        public async Task<IEnumerable<LoanDTO?>> GetUserLoanAsync(int id)
        {
            return await _loanRepository.GetUserLoanAsync(id);
        }

        public async Task<bool> VerifyBorrowAsync(int loanId, int issuedByUserId)
        {
            return await _loanRepository.VerifyBorrowAsync(loanId, issuedByUserId);
        }
    }
}