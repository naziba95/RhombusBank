using Microsoft.Extensions.Options;
using RhombusBank.API.Data;
using RhombusBank.API.Models;
using RhombusBank.API.Models.Domain;
using RhombusBank.API.Services.Interface;
using RhombusBank.API.Utils;
using System.Transactions;
using Transaction = RhombusBank.API.Models.Domain.Transaction;

namespace RhombusBank.API.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _OurBankSettlementAccount;
        private readonly IAccountService _accountService;
            
        public TransactionService(AppDbContext dbContext, ILogger<TransactionService> logger, 
            IOptions<AppSettings> settings, IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _OurBankSettlementAccount = _settings.OurBankSettlementAccount;
            _accountService = accountService;

        }


        public TransactionResponse CreateNewTransaction(Transaction transaction)
        {
           // Create a new transaction

            TransactionResponse response = new TransactionResponse(); // try using this to return a custom response for account service
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction Created successfully";
            response.Data = null;

            return response;

        }

        public TransactionResponse FindTransactionByDate(DateTime date)
        {
            TransactionResponse response = new TransactionResponse();
            var transactions = _dbContext.Transactions.Where(x => x.TransactionDate == date).ToList();

            return response;

        }

        public TransactionResponse MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
           // make a deposit
           TransactionResponse response = new TransactionResponse();
            Account sourceAccount;
            Account destintionAccount;
            Transaction transaction = new Transaction();

            // first, check that user - owner account is valid
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser != null) throw new ApplicationException("Invalid credentials");

            try
            {

            //Authorised user can now proceed to make a deposit: Take funds from settlement account angd give to customer
            sourceAccount = _accountService.GetByAccountNumber(_OurBankSettlementAccount);
            destintionAccount = _accountService.GetByAccountNumber(AccountNumber);

            // Update their balances
            sourceAccount.CurrentAccountBalance -= Amount;
            destintionAccount.CurrentAccountBalance += Amount;

            // Check if updates are successful
            if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                (_dbContext.Entry(destintionAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
            {
                //Then transaction is successful
                transaction.TransactionStatus = TranStatus.Success;
                response.ResponseCode = "00";
                response.ResponseMessage = "Transaction successfull";
                response.Data = null;
            }

            else
            {
                transaction.TransactionStatus = TranStatus.Failed;
                response.ResponseCode = "02";
                response.ResponseMessage = "Transaction Failed";
                response.Data = null;
            }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured => {ex.Message}");
            }

            // set other properties of transaction here
            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _OurBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactioAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {transaction.TransactionSourceAccount} TO DESTINATION => " +
                $"{transaction.TransactionDestinationAccount} FOR AMOUNT => {transaction.TransactioAmount} ON => " +
                $"{transaction.TransactionDate} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            // Commit to db
            _dbContext.Transactions.Add( transaction );
            _dbContext.SaveChanges();

            return response;

        }
     


        public TransactionResponse MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {

            // make a deposit
            TransactionResponse response = new TransactionResponse();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // first, check that user - owner account is valid
            var authUser = _accountService.Authenticate(FromAccount, TransactionPin);
            if (authUser != null) throw new ApplicationException("Invalid credentials");

            try
            {

                //Authorised user can now proceed to make a transfer: Take funds FromAccount and put in ToAccount
                sourceAccount = _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = _accountService.GetByAccountNumber(ToAccount);

                // Update their balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // Check if updates are successful
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //Then transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successfull";
                    response.Data = null;
                }

                else
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured => {ex.Message}");
            }

            // set other properties of transaction here
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = _OurBankSettlementAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactioAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {transaction.TransactionSourceAccount} TO DESTINATION => " +
                $"{transaction.TransactionDestinationAccount} FOR AMOUNT => {transaction.TransactioAmount} ON => " +
                $"{transaction.TransactionDate} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            // Commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;

        }

        public TransactionResponse MakeWithDrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {


            // make a withdrawal
            TransactionResponse response = new TransactionResponse();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // first, check that user - owner account is valid
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser != null) throw new ApplicationException("Invalid credentials");

            try
            {

                //Authorised user can now proceed to make a withdrawal: Take funds from customer and give to settlement account.
                destinationAccount = _accountService.GetByAccountNumber(_OurBankSettlementAccount);
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);

                // Update their balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // Check if updates are successful
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //Then transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successfull";
                    response.Data = null;
                }

                else
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured => {ex.Message}");
            }

            // set other properties of transaction here
            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = _OurBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactioAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {transaction.TransactionSourceAccount} TO DESTINATION => " +
                $"{transaction.TransactionDestinationAccount} FOR AMOUNT => {transaction.TransactioAmount} ON => " +
                $"{transaction.TransactionDate} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            // Commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;





        }

     
    }
}
