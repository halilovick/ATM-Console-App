using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp
{
    public class ATMScreen
    {
        internal const string currency = "EUR ";
        internal static void WelcomeScreen()
        {
            Console.Clear();
            Console.Title = "VVS Bank ATM";

            Console.WriteLine("\n-----------------Welcome to VVS Bank-----------------\n");
            Console.WriteLine("Please insert your card");
            Utility.PressEnterToContinue();
        }

        internal static User UserLogin()
        {
            User tempUser = new User();

            tempUser.CardNumber = Utility.Convert<long>("your card number.");
            tempUser.CardPin = Convert.ToInt32(Utility.GetPINInput("Enter your card PIN"));
            return tempUser;
        }

        internal static void LoginAnimation()
        {
            Console.WriteLine("\nChecking card number and PIN...");
            Utility.PrintDotAnimation();
        }

        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome {fullName}");
            Utility.PressEnterToContinue();
        }

        internal static void DisplayAppMenu()
        {
            Console.Clear();
            Console.WriteLine("-------VVS ATM menu-------");
            Console.WriteLine(":                           :");
            Console.WriteLine("1. View Balance             :");
            Console.WriteLine("2. Deposit Money            :");
            Console.WriteLine("3. Withdraw Money           :");
            Console.WriteLine("4. Transfer Money           :");
            Console.WriteLine("5. View Transactions        :");
            Console.WriteLine("6. Logout                   :");
        }

        internal static void LogoutCustomer()
        {
            Console.WriteLine("Thank you for using My ATM App.");
            Utility.PrintDotAnimation();
            Console.Clear();
        }
    }
}