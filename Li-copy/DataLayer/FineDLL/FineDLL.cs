using Dapper;
using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.Models.Fine;
using Li_copy.Models.Loans;
using Li_copy.Models.Users;
using System.Data;

namespace Li_copy.DataLayer.FineDLL
{
    public class FineDLL : IfineDLL
    {
        private readonly IDbConnection _dbConnection;

        public FineDLL(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
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
            string sql = @"
        SELECT f.*, l.UserId 
        FROM Fines f
        INNER JOIN Loans l ON f.LoanId = l.Id"; ;
            return await _dbConnection.QueryAsync<Fine>(sql);
        }

        public async Task<Fine?> GetFineByLoanIdAsync(int userId)
        {
            string sql = @"
        SELECT F.* FROM dbo.Fines F
        JOIN dbo.Loans L ON F.LoanId = L.Id
        WHERE L.UserId = @UserId 
        ORDER BY F.Id DESC"; // Add sorting to get a specific record

            return await _dbConnection.QueryFirstOrDefaultAsync<Fine>(sql, new {UserId =  userId });
        }
        public async Task<Fine?> GetFineByIdAsync(int loanId)
        {
            Console.WriteLine("reached databse");
            string sql = "SELECT * FROM Fines WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Fine>(sql, new { Id = loanId });
        }

        //public async Task<IEnumerable<Fine?>> GetFineByUserIdAsync(int userId)
        //{
        //    string sql = @"SELECT F.* FROM dbo.Fines F
        //JOIN dbo.Loans L ON F.LoanId = L.Id
        //WHERE L.UserId = @UserId
        //ORDER BY F.Id DESC";

        //    return await _dbConnection.QueryAsync<Fine>(sql, new { UserId = userId });
        //}

        //Task<Fine?> IfineDLL.GetFineByUserIdAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<IEnumerable<Fine?>> GetFineByUserIdAsync(int userId)
        {
            string sql = @"
        SELECT F.* FROM dbo.Fines F
        JOIN dbo.Loans L ON F.LoanId = L.Id
        WHERE L.UserId = @UserId
        ORDER BY F.Id DESC";

            return await _dbConnection.QueryAsync<Fine>(sql, new { UserId = userId });
        }

        public async Task<bool> UpdatePaymentInitializationAsync(int fineId, string transactionId, string requestJson, string responseJson)
        {
            const string sql = @"
                UPDATE Fines
                SET TransactionId = @TransactionId,
                    PaymentStatus = 'PENDING',
                    PaymentRequestJson = @Request,
                    PaymentInitResponseJson = @Response
                WHERE Id = @FineId";

            return await _dbConnection.ExecuteAsync(sql, new
            {
                FineId = fineId,
                TransactionId = transactionId,
                Request = requestJson,
                Response = responseJson
            }) > 0;
        }

        public async Task<Fine?> GetFineByTransactionIdAsync(string transactionId)
        {
            const string sql = "SELECT * FROM Fines WHERE TransactionId = @TransactionId";
            return await _dbConnection.QueryFirstOrDefaultAsync<Fine>(sql, new { TransactionId = transactionId });
        }

        public async Task<bool> MarkFinePaidAsync(string transactionId, string callbackJson, string verificationJson)
        {
            const string sql = @"
                UPDATE Fines
                SET IsPaid = 1,
                    PaidDate = GETDATE(),
                    PaymentStatus = 'COMPLETE',
                    PaymentCallbackJson = @CallbackJson,
                    PaymentFinalResponseJson = @VerificationJson
                WHERE TransactionId = @TransactionId";

            return await _dbConnection.ExecuteAsync(sql, new
            {
                TransactionId = transactionId,
                CallbackJson = callbackJson,
                VerificationJson = verificationJson
            }) > 0;
        }
    }
}


