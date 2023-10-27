using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp
{
    public class TransactionProcess
    {
        public long id { get; set; }
        public Decimal Amount { get; set; }
        public string Description { get; set; }
        public long UserBankAccountId { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
