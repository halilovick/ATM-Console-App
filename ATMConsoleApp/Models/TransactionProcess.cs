using System;

namespace ATMConsoleApp.Models
{
    public class TransactionProcess
    {
        public long id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public long UserBankAccountId { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
