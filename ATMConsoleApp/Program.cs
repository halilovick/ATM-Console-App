using ATMConsoleApp.Models;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ATMConsoleApp
{
    public class Program
    {
        public static List<User> userList;
        public static User selectedUser;
        public static List<TransactionProcess> _listOfTransactions;
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
            while (true)
            {
                ATMScreen.DisplayAppMenu();
                ProcessMenuChoice();
            }
        }

        public static string HashFunction(string pin)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] pinBytes = Encoding.UTF8.GetBytes(pin);
                byte[] hashBytes = sha256.ComputeHash(pinBytes);
                string pinHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return pinHash;
            }
        }

        public static void InitializeData()
        {
            userList = new List<User>
            {
                new User{Id=1, FullName = "Kerim Halilovic", AccountNumber=112233,CardNumber=111222, CardPin=HashFunction("1234"),AccountBalance=50000.00m},
                new User{Id=2, FullName = "Emir Kalajdzija", AccountNumber=445566,CardNumber=333444, CardPin=HashFunction("1234"),AccountBalance=4000.00m},
                new User{Id=3, FullName = "Emir Salkic", AccountNumber=778899,CardNumber=555666, CardPin=HashFunction("1234"),AccountBalance=2000.00m},
                new User{Id=4, FullName = "Bakir Pljakic", AccountNumber=115599,CardNumber=777888, CardPin=HashFunction("1234"),AccountBalance=30000.00m}
            };
            _listOfTransactions = new List<TransactionProcess>();
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
                    string provjera = HashFunction(selectedUser.CardPin);
                    inputAccount.CardPin = provjera;
                    if (inputAccount.CardNumber.Equals(selectedUser.CardNumber))
                    {
                        if (inputAccount.CardPin.Equals(provjera))
                        {
                            selectedUser = account;
                            isCorrectLogin = true;
                            break;
                        }
                    }
                }
                if (isCorrectLogin == false)
                {
                    Utility.PrintMessage("\nInvalid card number or PIN.", false);
                }
                Console.Clear();
            }
        }

        public static void ChangePin(string pin, int id)
        {
            foreach (User account in userList)
            {
                if (account.Id == id) account.CardPin = HashFunction(pin);
            }
        }

        private static void ProcessMenuChoice()
        {
            switch (Utility.Convert<int>("Enter an option:"))
            {
                case 1:
                    BalanceCheck();
                    break;
                case 2:
                    try
                    {
                        PlaceDeposit();
                    }
                    catch (Exception ex)
                    {
                        Utility.PrintMessage(ex.Message, false);
                    }
                    break;
                case 3:
                    try
                    {
                        MakeWithdrawal();
                    }
                    catch (Exception ex)
                    {
                        Utility.PrintMessage(ex.Message, false);
                    }
                    break;
                case 4:
                    var internalTransfer = screen.CreateInternalTransferTransaction();
                    try
                    {
                        ProcessInternalTransfer(internalTransfer);
                    }
                    catch (Exception ex)
                    {
                        Utility.PrintMessage(ex.Message, false);
                    }
                    break;
                case 5:
                    ViewTransaction();
                    break;
                case 6:
                    string pin = ATMScreen.ChangePIN();
                    ChangePin(pin, selectedUser.Id);
                    break;
                case 7:
                    ATMScreen.LogoutCustomer();
                    Utility.PrintMessage("You have successfully logged out. Please collect " +
                        "your ATM card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option.", false);
                    break;
            }
        }

        public static void BalanceCheck()
        {
            Utility.PrintMessage($"Your account balance is: {selectedUser.AccountBalance} EUR");
        }

        public static void PlaceDeposit(int amount = -1)
        {
            Console.WriteLine("\nOnly multiples of 5 and 10 euros allowed.\n");
            var transactionAmount = 0;
            if (amount == -1) 
            {
                transactionAmount = Utility.Convert<int>($"Enter an amount {ATMScreen.currency}");
                Console.WriteLine("\nChecking and counting bank notes.");
                Utility.PrintDotAnimation();
                Console.WriteLine("");

                if (!ConfirmDeposit(transactionAmount))
                {
                    Utility.PrintMessage($"You have cancelled your action.", false);
                    return;
                }
            } else
            {
                transactionAmount = amount;
            }

            if (transactionAmount <= 0 || transactionAmount > 2000)
            {
                throw new ArgumentException("Amount needs to be a number between 0 and 2000. Please try again.");
            }
            if (transactionAmount % 5 != 0)
            {
                throw new ArgumentException($"Enter deposit amount in multiples of 5 or 10. Please try again.");
            }

            CreateTransaction(selectedUser.Id, "Deposit", transactionAmount, "");

            selectedUser.AccountBalance += transactionAmount;

            if(amount == -1)
            {
                Utility.PrintMessage($"Your deposit of {transactionAmount} was " +
                $"succesful.", true, false);
                Utility.PrintDotAnimation();
            }
        }

        private static bool ConfirmDeposit(int amount)
        {
            Console.WriteLine("\nConfirm the deposit\n");
            Console.WriteLine("------");
            Console.WriteLine($"Total amount: {amount}\n\n");

            int opt = Utility.Convert<int>("Press 1 to confirm");
            return opt.Equals(1);
        }

        public static void MakeWithdrawal(int amount = -2)
        {
            var transactionAmount = 0;
            var selectedAmount = 0;
            if(amount == -2)
            {
                selectedAmount = ATMScreen.SelectWithdrawalAmount();
            } else
            {
                selectedAmount = amount;
            }
            if (selectedAmount == -1)
            {
                MakeWithdrawal();
                return;
            }
            else if (selectedAmount != 0)
            {
                transactionAmount = selectedAmount;
            }
            else
            {
                transactionAmount = Utility.Convert<int>($"Enter an amount {ATMScreen.currency}");
            }

            if (transactionAmount <= 0)
            {
                throw new ArgumentException("Amount needs to be greater than zero. Try again");
            }
            if (transactionAmount % 5 != 0)
            {
                throw new ArgumentException("You can only withdraw amount in multiples of 5 or 10 euros. Please try again.");
            }

            if (transactionAmount > selectedUser.AccountBalance)
            {
                throw new ArgumentException($"Withdrawal failed. Your balance is too low to withdraw " +
                    $"{transactionAmount}");
            }
            CreateTransaction(selectedUser.Id, "Withdrawal", -transactionAmount, "");
            selectedUser.AccountBalance -= transactionAmount;
            if(amount == -2)
            {
                Utility.PrintMessage($"You have successfully withdrawn {transactionAmount}", true, false);
                Utility.PrintDotAnimation();
            }
        }

        private static void ProcessInternalTransfer(InternalTransferTransaction internalTransfer)
        {
            if (internalTransfer.Amount <= 0)
            {
                throw new ArgumentException("Transfer amount needs to be greater than zero. Please try again.");
            }
            if (internalTransfer.Amount > selectedUser.AccountBalance)
            {
                throw new ArgumentException($"Transfer failed. Your balance is not enough" +
                    $" to transfer {internalTransfer.Amount}");
            }

            var selectedReciever = (from userAcc in userList
                                    where userAcc.AccountNumber == internalTransfer.RecipientAccountNumber
                                    select userAcc).FirstOrDefault();
            if (selectedReciever == null)
            {
                throw new ArgumentException("Transfer failed. Reciever bank account number is invalid.");
            }
            if (selectedReciever.FullName != internalTransfer.RecipientAccountName)
            {
                throw new ArgumentException("Transfer Failed. Recipient's bank account name does not match.");
            }

            CreateTransaction(selectedUser.Id, "Transfer", -internalTransfer.Amount, "Transfered " +
                $"to {selectedReciever.AccountNumber} ({selectedReciever.FullName})");

            selectedUser.AccountBalance -= internalTransfer.Amount;

            CreateTransaction(selectedReciever.Id, "Transfer", internalTransfer.Amount, "Transfered from " +
                $"{selectedUser.AccountNumber}({selectedUser.FullName})");
            selectedReciever.AccountBalance += internalTransfer.Amount;
            Utility.PrintMessage($"You have successfully transfered" +
                $" {internalTransfer.Amount} to " +
                $"{internalTransfer.RecipientAccountName}", true);

        }

        private static void sortTableByAmount()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedUser.Id).ToList();
            var sortedTransactionList = filteredTransactionList.OrderByDescending(transaction => transaction.Amount).ToList();
            var sortedTable = new ConsoleTable("Id", "Transaction Date", "Type", "Description", "Amount " + ATMScreen.currency);
            foreach (var transaction in sortedTransactionList)
            {
                sortedTable.AddRow(transaction.id, transaction.TransactionDate, transaction.TransactionType, transaction.Description, transaction.Amount);
            }
            sortedTable.Options.EnableCount = false;
            Console.WriteLine("");
            sortedTable.Write();
        }

        private static void sortTableByType()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedUser.Id).ToList();
            var sortedTransactionList = filteredTransactionList.OrderBy(transaction => transaction.TransactionType).ToList();
            var sortedTable = new ConsoleTable("Id", "Transaction Date", "Type", "Description", "Amount " + ATMScreen.currency);
            foreach (var transaction in sortedTransactionList)
            {
                sortedTable.AddRow(transaction.id, transaction.TransactionDate, transaction.TransactionType, transaction.Description, transaction.Amount);
            }
            sortedTable.Options.EnableCount = false;
            sortedTable.Write();
        }

        public static void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedUser.Id).ToList();
            if (filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Description", "Amount " + ATMScreen.currency);
                foreach (var transaction in filteredTransactionList)
                {
                    table.AddRow(transaction.id, transaction.TransactionDate, transaction.TransactionType, transaction.Description, transaction.Amount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)\n", true, false);
                Console.WriteLine("Press 1 to sort by Amount, 2 to sort by Type, 3 to continue...");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("\nSorting by amount...");
                        Utility.PrintDotAnimation(false);
                        sortTableByAmount();
                        Utility.PressEnterToContinue();
                        break;
                    case "2":
                        Console.WriteLine("\nSorting by type...");
                        Utility.PrintDotAnimation(false);
                        sortTableByType();
                        Utility.PressEnterToContinue();
                        break;
                    default:
                        break;
                }
            }
        }

        public static void CreateTransaction(long userBankAccountId, string transactionType, decimal transactionAmount, string description)
        {
            var transaction = new TransactionProcess()
            {
                id = Utility.GetTransactionId(),
                Amount = transactionAmount,
                Description = description,
                UserBankAccountId = userBankAccountId,
                TransactionType = transactionType,
                TransactionDate = DateTime.Now
            };

            _listOfTransactions.Add(transaction);
        }
    }
}
