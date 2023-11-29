namespace ATMConsoleApp.Models
{
    public class InternalTransferTransaction
    {
        public decimal Amount { get; set; }
        public string RecipientAccountName { get; set; }
        public long RecipientAccountNumber { get; set; }
        public InternalTransferTransaction(decimal amount, string recipientAccountName, long recipientAccountNumber)
        {
            Amount = amount;
            RecipientAccountName = recipientAccountName;
            RecipientAccountNumber = recipientAccountNumber;
        }
        public InternalTransferTransaction()
        {
        }
    }
}
