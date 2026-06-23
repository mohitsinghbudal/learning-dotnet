using Li_copy.Models.Fine;
using Li_copy.I_InterfaceLayer.FineInterface;
using System.Data;
using Dapper;

namespace Li_copy.DataLayer.FineDLL
{
    public class FineDLL : IfineDLL
    {
        private readonly IDbConnection _dbConnection;

        public FineDLL(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Fine?> GetFineByLoanIdAsync(int loanId)
        {
            string sql = "SELECT * FROM Fines WHERE LoanId = @LoanId";
            return await _dbConnection.QueryFirstOrDefaultAsync<Fine>(sql, new { LoanId = loanId });
        }

        public async Task<int> CreateFineAsync(Fine fine)
        {
            string sql = @"
                INSERT INTO Fines (LoanId, Amount, IsPaid, PaidDate, TransactionId, PaymentStatus, 
                                   PaymentRequestJson, PaymentInitResponseJson, PaymentCallbackJson, PaymentFinalResponseJson)
                VALUES (@LoanId, @Amount, @IsPaid, @PaidDate, @TransactionId, @PaymentStatus, 
                        @PaymentRequestJson, @PaymentInitResponseJson, @PaymentCallbackJson, @PaymentFinalResponseJson);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _dbConnection.ExecuteScalarAsync<int>(sql, fine);
        }

        public async Task<bool> UpdateFineAsync(Fine fine)
        {
            string sql = @"
                UPDATE Fines
                SET LoanId = @LoanId, Amount = @Amount, IsPaid = @IsPaid, PaidDate = @PaidDate, 
                    TransactionId = @TransactionId, PaymentStatus = @PaymentStatus, 
                    PaymentRequestJson = @PaymentRequestJson, PaymentInitResponseJson = @PaymentInitResponseJson, 
                    PaymentCallbackJson = @PaymentCallbackJson, PaymentFinalResponseJson = @PaymentFinalResponseJson
                WHERE Id = @Id";
            int rows = await _dbConnection.ExecuteAsync(sql, fine);
            return rows > 0;
        }

        public async Task<IEnumerable<Fine>> GetAllFinesAsync()
        {
            string sql = "SELECT * FROM Fines";
            return await _dbConnection.QueryAsync<Fine>(sql);
        }

        public async Task<Fine?> GetFineByIdAsync(int id)
        {
            string sql = "SELECT * FROM Fines WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Fine>(sql, new { Id = id });
        }
    }
}

