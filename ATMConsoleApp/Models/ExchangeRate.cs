using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp.Models
{
    public class ExchangeRate
    {
        public string CurrencyPair { get; set; }
        public double Rate { get; set; } = 0;
    }
}
