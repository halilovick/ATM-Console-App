using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace ATMConsoleApp
{
    class Program
    {
        private static List<User> userList;
        private static User selectedUser;
        private static List<Transaction> _listOfTransactions;
        private static ATMScreen screen;
        static void Main()
        {
            screen = new ATMScreen();
            InitializeData();
            Run();
        }
        public static void Run()
        {
            ATMScreen.WelcomeScreen();
            CheckUserCredentials();
            ATMScreen.WelcomeCustomer(selectedUser.FullName);
            ATMScreen.DisplayAppMenu();
        }
        public static void InitializeData()
        {
            userList = new List<User>
            {
                new User{Id=1, FullName = "Kerim Halilovic", AccountNumber=112233,CardNumber=111222, CardPin=1234,AccountBalance=50000.00m},
                new User{Id=2, FullName = "Emir Kalajdzija", AccountNumber=445566,CardNumber=333444, CardPin=1234,AccountBalance=4000.00m},
                new User{Id=3, FullName = "Emir Salkic", AccountNumber=778899,CardNumber=555666, CardPin=1234,AccountBalance=2000.00m},
                new User{Id=4, FullName = "Bakir Pljakic", AccountNumber=115599,CardNumber=777888, CardPin=1234,AccountBalance=30000.00m}
            };
            _listOfTransactions = new List<Transaction>();
        }

        public static void CheckUserCredentials()
        {
            bool isCorrectLogin = false;
            while (!isCorrectLogin)
            {
                User inputAccount = ATMScreen.UserLogin();
                ATMScreen.LoginAnimation();
                foreach (User account in userList)
                {
                    selectedUser = account;
                    if (inputAccount.CardNumber.Equals(selectedUser.CardNumber))
                    {

                        if (inputAccount.CardPin.Equals(selectedUser.CardPin))
                        {
                            selectedUser = account;
                            isCorrectLogin = true;
                            break;
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\nInvalid card number or PIN.", false);
                    }
                    Console.Clear();
                }
            }
        }

    }
}
