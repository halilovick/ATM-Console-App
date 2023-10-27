using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp
{
    public interface ITransaction
    {
        //void insert(long userBankAccountId, TransactionType transType, decimal transAmount, string desc);
        void view();
    }
}
