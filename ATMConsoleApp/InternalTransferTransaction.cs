using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp
{
    public class InternalTransferTransaction
    {
        public decimal Amount { get; set; } 
        public string RecipientAccountName { get; set; }
        public long RecipientAccountNumber { get; set; }
    }
}
