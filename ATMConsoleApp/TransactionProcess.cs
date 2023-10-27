using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp
{
    public class TransactionProcess
    {
        public Decimal Amount { get; set; }
        public long id { get; set; }
        public string Desrciption { get; set; }
        public long UserBankAccountId { get; set; }
        //public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
