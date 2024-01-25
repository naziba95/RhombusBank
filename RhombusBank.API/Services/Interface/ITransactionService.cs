using RhombusBank.API.Models;
using RhombusBank.API.Models.Domain;
using System.Transactions;
using Transaction = RhombusBank.API.Models.Domain.Transaction;

namespace RhombusBank.API.Services.Interface
{
    public interface ITransactionService
    {
        TransactionResponse CreateNewTransaction(Transaction transaction);
        TransactionResponse FindTransactionByDate(DateTime date);
        TransactionResponse MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin);
        TransactionResponse MakeWithDrawal(string AccountNumber, decimal Amount, string TransactionPin);
        TransactionResponse MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount,  string TransactionPin);
    }
}
