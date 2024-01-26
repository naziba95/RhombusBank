using RhombusBank.API.Models.Domain;

namespace RhombusBank.API.Models.DTO
{
    public class TransactionDto
    {
        public string TransactionUniqueReference { get; set; }
        public decimal TransactioAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }
        public bool IsSuccessful => TransactionStatus.Equals(TranStatus.Success);
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public string TransactionParticulars { get; set; }
        public TranType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
