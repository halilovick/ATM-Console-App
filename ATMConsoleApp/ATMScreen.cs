﻿using ATMConsoleApp.Models;
using System;
using System.Collections.Generic;

namespace ATMConsoleApp
{
    public class ATMScreen
    {
        internal const string currency = "EUR ";
        public static void WelcomeScreen(bool clearConsole = true)
        {
            if (clearConsole)
            {
                Console.Clear();
            }
            Console.Title = "VVS Bank ATM";

            Console.WriteLine("\n-----------------Welcome to VVS Bank-----------------\n");
            Console.WriteLine("Please insert your card");
            Utility.PressEnterToContinue();
        }

        public static User UserLogin(List<ConsoleKeyInfo> consoleKeys = null)
        {
            User tempUser = new User();

            tempUser.CardNumber = Utility.Convert<long>("Enter your card number.");
            tempUser.CardPin = Convert.ToString(Utility.GetPINInput("Enter your card PIN", consoleKeys));
            Program.HashFunction(tempUser.CardPin);
            return tempUser;
        }

        public static void LoginAnimation()
        {
            Console.WriteLine("\nChecking card number and PIN...");
            Utility.PrintDotAnimation();
        }

        public static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome {fullName}");
            Utility.PressEnterToContinue();
        }

        internal static string ChangePIN()
        {
            return Convert.ToString(Utility.GetPINInput("Enter your new PIN"));
        }

        public static void DisplayAccountInformation(string fullName, long cardNumber, long accountNumber)
        {
            Console.WriteLine("\nYour account information: ");
            Console.WriteLine($"Name: {fullName}");
            Console.WriteLine($"Account number: {accountNumber}");
            Console.WriteLine($"Card number: {cardNumber}");
        }


        public static void DisplayAppMenu(bool clearConsole = true)
        {
            if (clearConsole)
            {
                Console.Clear();
            }
            Console.WriteLine("--------------VVS ATM menu--------------");
            Console.WriteLine(":                                      :");
            Console.WriteLine(": 1. View Balance                      :");
            Console.WriteLine(": 2. Deposit Money                     :");
            Console.WriteLine(": 3. Withdraw Money                    :");
            Console.WriteLine(": 4. Transfer Money                    :");
            Console.WriteLine(": 5. View Transactions                 :");
            Console.WriteLine(": 6. View Account Information          :");
            Console.WriteLine(": 7. Exchange rate                     :");
            Console.WriteLine(": 8. Logout                            :");
            Console.WriteLine(":                                      :");
            Console.WriteLine("----------------------------------------");
        }

        public static void LogoutCustomer()
        {
            Console.WriteLine("Thank you for using our ATM!");
            Utility.PrintDotAnimation();
        }

        public static int SelectWithdrawalAmount()
        {
            Console.WriteLine("");
            Console.WriteLine("------------------------------");
            Console.WriteLine(":                            :");
            Console.WriteLine(": 1.{0}10       2.{0}20    :", currency);
            Console.WriteLine(": 3.{0}50       4.{0}100   :", currency);
            Console.WriteLine(": 5.{0}500      6.{0}1000  :", currency);
            Console.WriteLine(": 7.{0}5000     8.{0}10000 :", currency);
            Console.WriteLine(": 0.Other                    :");
            Console.WriteLine(":                            :");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            int selectedAmount = Utility.Convert<int>("Enter an option:");
            switch (selectedAmount)
            {
                case 1:
                    return 10;
                case 2:
                    return 20;
                case 3:
                    return 50;
                case 4:
                    return 100;
                case 5:
                    return 500;
                case 6:
                    return 1000;
                case 7:
                    return 5000;
                case 8:
                    return 10000;
                case 0:
                    return 0;
                default:
                    Utility.PrintMessage("Invalid input. Try again.", false);
                    return -1;
            }
        }

        public InternalTransferTransaction CreateInternalTransferTransaction()
        {
            var internalTransfer = new InternalTransferTransaction();
            internalTransfer.RecipientAccountNumber = Utility.Convert<long>("Enter recipient's account number:");
            internalTransfer.Amount = Utility.Convert<decimal>($"amount {currency}");
            internalTransfer.RecipientAccountName = Utility.GetUserInput("Enter recipient's name:");
            return internalTransfer;
        }
    }
}