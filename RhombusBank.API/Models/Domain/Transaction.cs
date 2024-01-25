using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace RhombusBank.API.Models.Domain
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionUniqueReference { get; set; }
        public decimal TransactioAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }
        public bool IsSuccessful => TransactionStatus.Equals(TranStatus.Success);
        public string TransactionSourceAccount { get; set; }    
        public string TransactionDestinationAccount { get; set;}
        public string TransactionParticulars { get; set; }  
        public TranType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

        public Transaction()
        {
            //TransactionDate = DateTime.Now;

            TransactionUniqueReference = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";
        }

    }

    public enum TranStatus{
        Failed,
        Success,
        Pending,
        Error
    }

    public enum TranType 
    { 
        Deposit,
        Withdrawal,
        Transfer
    }
}
