using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp
{
    public static class AppValidator
    {
        public static T Convert<T>(string value)
        {
            bool valid = false;
            string input;

            while(!valid) { 
                input = Utility.GetUserInput(value);

                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        return (T)converter.ConvertFromString(input);
                    } else
                    {
                        return default;
                    }
                }
                catch
                {
                    Utility.PrintMessage("Your input is not valid. Please try again.", false);
                }
            }
            return default;
        }
    }
}
