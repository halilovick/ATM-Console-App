using ATMConsoleApp.Models;
using ATMConsoleApp;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.IO;
using System;
using NUnit.Framework.Constraints;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections;
using System.Formats.Asn1;
using CsvHelper;
using System.Globalization;
using System.Linq;
using System.Xml;
using NUnit.Framework.Internal;
using Moq;

namespace TestProject1
{
    public class Tests
    {
        public interface MockInterface
        {
            int Id { get; set; }
            string FullName {  get; set; }
            string CardPin { get; set; }

            long CardNumber { get; set; }

            decimal AccountBalance {  get; set; }

            long AccountNumber {  get; set; }

        }

        public class Mock : MockInterface
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string CardPin { get; set; }
            public long CardNumber { get; set; }

            public decimal AccountBalance { get; set; }
            public long AccountNumber { get; set; }
            
        }
        [SetUp]
        public void Setup()
        {
            Program.InitializeData();
        }

        [Test]
        public void ChangePinMethodCheck()
        {
            Program.selectedUser = Program.userList[0];
            Program.ChangePin("6666", Program.selectedUser.Id);
            Assert.Pass(Program.selectedUser.CardPin, Program.HashFunction("6666"));
            Program.selectedUser = Program.userList[1];
            Program.ChangePin("9191", Program.selectedUser.Id);
            Assert.Pass(Program.selectedUser.CardPin, Program.HashFunction("9191"));
        }

        //Zamjenski objekat
        [Test]
        public void ChangePinMethodMockTest()
        {
            var mockUser = new Mock<Mock>();
            mockUser.Object.CardPin = "1234";
            mockUser.Object.Id = 1;
            Program.ChangePin("6666", mockUser.Object.Id);
            Assert.Pass(mockUser.Object.CardPin, Program.HashFunction("6666"));
        }

        [Test]
        public void CreateTransactionTest()
        {
            var numberOfTransactions = Program._listOfTransactions.Count;
            Program.selectedUser = Program.userList[0];
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 100, "Deposit test");
            Assert.That(numberOfTransactions + 1, Is.EqualTo(Program._listOfTransactions.Count));
            Assert.That(Program._listOfTransactions[numberOfTransactions].TransactionType, Is.EqualTo("Deposit"));
            Assert.That(Program._listOfTransactions[numberOfTransactions].Amount, Is.EqualTo(100));
        }
        // Zamjenski objekat
        [Test]
        public void CreateTransactionMockTest()
        {
            var mockUser = new Mock<Mock>();
            mockUser.Object.CardPin = "1234";
            mockUser.Object.Id = 1;

            Program.CreateTransaction(mockUser.Object.Id, "Withdrawal", 50, "Withdrawal transaction");

            Assert.AreEqual(1, Program._listOfTransactions.Count);
            var transaction = Program._listOfTransactions[0];
            Assert.AreEqual(mockUser.Object.Id, transaction.UserBankAccountId);
            Assert.AreEqual("Withdrawal", transaction.TransactionType);
            Assert.AreEqual(50, transaction.Amount);
            Assert.AreEqual("Withdrawal transaction", transaction.Description);
        }


        //Zamjenski objekat
        [Test]
        public void ViewAccountInformationMockTest()
        {
    
            var mockUser = new Mock<Mock>();
            mockUser.Object.FullName = "Mock Test";
            mockUser.Object.CardNumber = 252525;
            mockUser.Object.AccountNumber = 121212;
            var output = new StringWriter();
            Console.SetOut(output);
            ATMScreen.DisplayAccountInformation(mockUser.Object.FullName, mockUser.Object.CardNumber, mockUser.Object.AccountNumber);
            Assert.That(output.ToString().Contains("Mock Test"));
            Assert.That(output.ToString().Contains("252525"));
            Assert.That(output.ToString().Contains("121212"));

        }

        //Zamjenski objekat
        [Test]
        public void WelcomeCustomerMockTest()
        {
            var mockUser = new Mock<Mock>();
            mockUser.Object.FullName = "Mock Test";
            var output = new StringWriter();
            Console.SetOut(output);
            ATMScreen.WelcomeCustomer(mockUser.Object.FullName);
            Assert.That(output.ToString().Contains("Welcome Mock Test"));

        }


        [Test]
        public void WithdrawMoneyTest()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeWithdraw = Program.selectedUser.AccountBalance;
            Program.MakeWithdrawal(100);
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeWithdraw - 100));
        }

        [Test]
        public void WithdrawMoneyExceptionTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 1000;
            var ex = Assert.Throws<System.ArgumentException>(() => Program.MakeWithdrawal(1100));
            Assert.That(ex.Message, Is.EqualTo("Withdrawal failed. Your balance is too low to withdraw 1100"));
        }

        [Test]
        public void WithdrawMoneyExceptionTest2()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 1000;
            var ex = Assert.Throws<System.ArgumentException>(() => Program.MakeWithdrawal(249));
            Assert.That(ex.Message, Is.EqualTo("You can only withdraw amount in multiples of 5 or 10 euros. Please try again."));
        }

        [Test]
        public void ConfirmDepositTrueTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("1");
            Console.SetIn(input);
            bool depositConfirm = Program.ConfirmDeposit(100);
            Assert.IsTrue(depositConfirm);
        }

        [Test]
        public void ConfirmDepositFalseTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("2");
            Console.SetIn(input);
            bool depositConfirm = Program.ConfirmDeposit(50);
            Assert.IsFalse(depositConfirm);
        }

        [Test]
        public void sortTableByAmountTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 100, "Deposit 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 50, "Deposit 2");
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 200, "Deposit 3");
            var output = new StringWriter();
            Console.SetOut(output);
            var sortedTable = Program.sortTableByAmount().ToString();
            Assert.That(output.ToString(), Is.EqualTo("\r\n" + sortedTable + "\r\n"));
        }

        [Test]
        public void sortTableByTypeTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 100, "Deposit 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Withdraw", 50, "Withdraw 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 200, "Deposit 2");
            var output = new StringWriter();
            Console.SetOut(output);
            var sortedTable = Program.sortTableByType().ToString();
            Assert.That(output.ToString(), Is.EqualTo(sortedTable + "\r\n"));
        }

         [Test]
         public void CheckUserCredentialsTest()
         {
             Program.selectedUser = Program.userList[0];
             var output = new StringWriter();
             Console.SetOut(output);
             List<ConsoleKeyInfo> consoleKeys = new List<ConsoleKeyInfo>{
                 new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false),
                 new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false),
                 new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false),
                 new ConsoleKeyInfo('4', ConsoleKey.D4, false, false, false),
                 new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)
             };
             var input = new StringReader("111222\n");
             Console.SetIn(input);
             var provjera = Program.CheckUserCredentials(consoleKeys);
             Assert.IsTrue(provjera);
         }

        [Test]
        public void UserLoginTest()
        {
            List<ConsoleKeyInfo> consoleKeys = new List<ConsoleKeyInfo>{
                new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false),
                new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false),
                new ConsoleKeyInfo('4', ConsoleKey.D4, false, false, false),
                new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)
            };
            var input = new StringReader("111222\n");
            Console.SetIn(input);
            var user = ATMScreen.UserLogin(consoleKeys);
            Assert.That(user.CardNumber, Is.EqualTo(111222));
            Assert.That(user.CardPin, Is.EqualTo("1234"));
        }

        [Test]
        public void BalanceCheckTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 1000;
            var output = new StringWriter();
            Console.SetOut(output);
            var input = new StringReader("");
            Console.SetIn(input);
            Program.BalanceCheck();
            string[] lines = output.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None);
            string firstLine = lines.Length > 0 ? lines[0] : string.Empty;
            Assert.That(firstLine, Is.EqualTo("Your account balance is: 1000 EUR"));
        }

        //White box - ViewTransaction
        //TC1: transaction: none
        [Test]
        public void ViewTransactionsEmptyTest()
        {
            Program.selectedUser = Program.userList[0];
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ViewTransaction();
            string[] lines = output.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None);
            string firstLine = lines.Length > 0 ? lines[0] : string.Empty;
            Assert.That(firstLine.ToString(), Is.EqualTo("You have no transaction yet."));
        }

        //TC2: transaction: existing, no sorting
        [Test]
        public void ViewTransactionsTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 100, "Deposit 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Withdraw", 50, "Withdraw 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 200, "Deposit 2");
            var output = new StringWriter();
            Console.SetOut(output);
            var input = new StringReader("3");
            Console.SetIn(input);
            var table = Program.ViewTransaction().ToString();
            string[] lines = table.Split(new[] { "\r\n" }, StringSplitOptions.None);
            StringBuilder concatenatedLines = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                concatenatedLines.Append(lines[i]);
                if (i < 9)
                {
                    concatenatedLines.Append(Environment.NewLine);
                }
            }
            Assert.That(concatenatedLines.ToString(), Is.EqualTo(table));
        }

        //TC3: transaction:existing, sorting by amount
        [Test]
        public void ViewTransactionsSortAmountTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 100, "Deposit 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Withdraw", 50, "Withdraw 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 200, "Deposit 2");
            var output = new StringWriter();
            Console.SetOut(output);
            var input = new StringReader("1");
            Console.SetIn(input);
            var table = Program.ViewTransaction().ToString();
            string[] lines = table.Split(new[] { "\r\n" }, StringSplitOptions.None);
            StringBuilder concatenatedLines = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                concatenatedLines.Append(lines[i]);
                if (i < 9)
                {
                    concatenatedLines.Append(Environment.NewLine);
                }
            }
            Assert.That(concatenatedLines.ToString(), Is.EqualTo(table));
        }

        //TC4: transaction:existing, sorting by type
        [Test]
        public void ViewTransactionsSortTypeTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 100, "Deposit 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Withdraw", 50, "Withdraw 1");
            Program.CreateTransaction(Program.selectedUser.Id, "Deposit", 200, "Deposit 2");
            var output = new StringWriter();
            Console.SetOut(output);
            var input = new StringReader("2");
            Console.SetIn(input);
            var table = Program.ViewTransaction().ToString();
            string[] lines = table.Split(new[] { "\r\n" }, StringSplitOptions.None);
            StringBuilder concatenatedLines = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                concatenatedLines.Append(lines[i]);
                if (i < 9)
                {
                    concatenatedLines.Append(Environment.NewLine);
                }
            }
            Assert.That(concatenatedLines.ToString(), Is.EqualTo(table));
        }


        [Test]
        public void InternalTransferExceptionTest1()
        {
            Program.selectedUser = Program.userList[0];
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(-10, "Test Name", 111222);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
            Assert.That(ex.Message, Is.EqualTo("Transfer amount needs to be greater than zero. Please try again."));
        }

        [Test]
        public void InternalTransferExceptionTest2()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 100;
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Test Name", 111222);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
            Assert.That(ex.Message, Is.EqualTo("Transfer failed. Your balance is not enough to transfer 500"));
        }

        [Test]
        public void InternalTransferExceptionTest3()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 1000;
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Kerim Halilovic", 0);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
            Assert.That(ex.Message, Is.EqualTo("Transfer failed. Reciever bank account number is invalid."));
        }

        [Test]
        public void InternalTransferExceptionTest4()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 1000;
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Invalid Name", 112233);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
            Assert.That(ex.Message, Is.EqualTo("Transfer Failed. Recipient's bank account name does not match."));
        }

        [Test]
        public void InternalTransferTest()
        {
            Program.selectedUser = Program.userList[1];
            Program.selectedUser.AccountBalance = 1000;
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Kerim Halilovic", 112233);
            Program.ProcessInternalTransfer(internalTransferTransaction);
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(500));
        }

        [Test]
        public void SelectWithdrawalAmountTest()
        {
            var input = new StringReader("1");
            Console.SetIn(input);
            int selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(10));
            input = new StringReader("2");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(20));
            input = new StringReader("3");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(50));
            input = new StringReader("4");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(100));
            input = new StringReader("5");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(500));
            input = new StringReader("6");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(1000));
            input = new StringReader("7");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(5000));
            input = new StringReader("8");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(10000));
            input = new StringReader("0");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(0));
            input = new StringReader("9");
            Console.SetIn(input);
            selectedAmount = ATMScreen.SelectWithdrawalAmount();
            Assert.That(selectedAmount, Is.EqualTo(-1));
        }

        [Test]
        public void LogoutCustomerTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            ATMScreen.LogoutCustomer();
            string[] lines = output.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None);
            string firstLine = lines.Length > 0 ? lines[0] : string.Empty;
            Assert.That(firstLine.ToString(), Is.EqualTo("Thank you for using our ATM!"));
        }

        [Test]
        public void CreateInternalTransferTest()
        {
            var input = new StringReader("112233\n100\nKerim Halilovic");
            Console.SetIn(input);
            ATMScreen screen = new ATMScreen();
            var internalTransferTransaction = screen.CreateInternalTransferTransaction();
            Assert.That(internalTransferTransaction.RecipientAccountNumber, Is.EqualTo(112233));
        }

        [Test]
        public void InvalidInputConvertTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            Utility.Convert<int>("InvalidPrompt\n");
           Assert.That(output.ToString().Contains("Invalid input. Try again."));
        } 

        [Test]
        public void BalanceCheckMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 1000;
            var input = new StringReader("1");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("Your account balance is: 1000 EUR"));
        }

        [Test]
        public void PlaceDepositMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("2\n100\n1");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("Only multiples of 5 and 10 euros allowed."));
            input = new StringReader("2\n101\n1");
            Console.SetIn(input);
            output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("Enter deposit amount in multiples of 5 or 10. Please try again."));
        }

        [Test]
        public void WithdrawMoneyMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 100;
            var input = new StringReader("3\n1");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("You have successfully withdrawn 10"));
            input = new StringReader("3\n5");
            Console.SetIn(input);
            output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("Withdrawal failed. Your balance is too low to withdraw 500"));
        }

        [Test]
        public void ProcessTransferMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            Program.selectedUser.AccountBalance = 100;
            var input = new StringReader("4\n112233\n100\nKerim Halilovic");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("You have successfully transfered 100 to Kerim Halilovic"));
            input = new StringReader("4\n112233\n-5\nKerim Halilovic\n");
            Console.SetIn(input);
            output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("Transfer amount needs to be greater than zero. Please try again."));
        }

        [Test]
        public void ViewTransactionMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            Program._listOfTransactions = new List<TransactionProcess>();
            var input = new StringReader("5\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("You have no transaction yet."));
        }

        [Test]
        public void ConvertCurrencyMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("7\n100\n1\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("Converted amount: 108"));
        }

        [Test]
        public void LogoutCustomerMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("8\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice(false);
            Assert.That(output.ToString().Contains("Thank you for using our ATM!"));
        }

        [Test]
        public void InvalidOptionMenuChoiceTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("100\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice(false);
            Assert.That(output.ToString().Contains("Invalid Option."));
        }

        [Test]
        public void DisplayAppMenuTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            ATMScreen.DisplayAppMenu(false);
            Assert.That(output.ToString().Contains("-------VVS ATM menu-------"));
        }

        [Test]
        public void ConsoleMethodsTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var input = new StringReader("\n");
            Console.SetIn(input);
            ATMScreen.WelcomeCustomer("Test");
            ATMScreen.LoginAnimation();
            input = new StringReader("\n");
            Console.SetIn(input);
            ATMScreen.WelcomeScreen(false);
            Assert.That(output.ToString().Contains("Welcome Test"));
            Assert.That(output.ToString().Contains("Checking card number and PIN..."));
            Assert.That(output.ToString().Contains("Please insert your card"));
        }

        [Test]
        public void GetPINInputTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            List<ConsoleKeyInfo> consoleKeys = new List<ConsoleKeyInfo>{
                new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false),
                new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false),
                new ConsoleKeyInfo('4', ConsoleKey.D4, false, false, false),
                new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)
            };
            var pin = Utility.GetPINInput("Enter your card PIN", consoleKeys);
            Assert.That(pin, Is.EqualTo("1234"));
        }

        [Test]
        public void GetPINInputBackspaceTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            List<ConsoleKeyInfo> consoleKeys = new List<ConsoleKeyInfo>{
                new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false),
                new ConsoleKeyInfo('4', ConsoleKey.D4, false, false, false),
                new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)
            };
            var pin = Utility.GetPINInput("Enter your card PIN", consoleKeys);
            Assert.That(pin, Is.EqualTo("1234"));
        }

        [Test]
        public void GetPINInputDigitsTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            List<ConsoleKeyInfo> consoleKeys = new List<ConsoleKeyInfo>{
                new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false),
                new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false),
                new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)
            };
            var pin = Utility.GetPINInput("Enter your card PIN", consoleKeys);
            //Assert.That(pin, Is.EqualTo("1234"));
            Assert.That(output.ToString().Contains("Please enter 4 digits."));
        }

        //Test Driven Development
        [Test]
        public void ViewAccountInformationTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("6\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ProcessMenuChoice();
            Assert.That(output.ToString().Contains("\nYour account information: "));
            Assert.That(output.ToString().Contains($"Name: {Program.selectedUser.FullName}"));
            Assert.That(output.ToString().Contains($"Account number: {Program.selectedUser.AccountNumber}"));
            Assert.That(output.ToString().Contains($"Card number: {Program.selectedUser.CardNumber}"));
        }

        [Test]
        public void ViewAccountInformationOptionTest()
        {
            Program.selectedUser = Program.userList[0];
            var input = new StringReader("2");
            Console.SetIn(input);
            Program.ViewAccountInformation();
            var output = new StringWriter();
            Console.SetOut(output);
            ATMScreen.DisplayAppMenu(false);
            Assert.That(output.ToString().Contains("-------VVS ATM menu-------"));
        }
        [Test]
        public void DisplayExchangeRateTest()
        {
            Program.selectedUser = Program.userList[1];
            var input = new StringReader("7\n");
            Console.SetIn(input);
            List<string> expectedExchangeRates = new List<string>
            {
                "EUR to USD: 1.08",
                "EUR to AUD: 1.65",
                "EUR to BAM: 1.96",
                "EUR to CAD: 1.47",
                "EUR to DKK: 7.46",
                "EUR to HUF: 379.16",
                "EUR to JPY: 159.13",
                "EUR to NOK: 11.77",
                "EUR to SEK: 11.29",
                "EUR to CHF: 0.95",
                "EUR to GBP: 0.86",
                "EUR to RUB: 99.55",
                "EUR to CNY: 7.72",
                "EUR to RSD: 117.22"
            };
            var output = new StringWriter();
            Console.SetOut(output);
            Program.DisplayExchangeRate();
            Assert.That(output.ToString().Contains("\nExchange rate"));
            foreach (var expectedRate in expectedExchangeRates)
            {
                Assert.That(output.ToString().Contains(expectedRate));
            }
        }

        [Test]
        public void ConvertCurrencyTest()
        {
            var input = new StringReader("100\n3\n"); 
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.ConvertCurrency();
            string outputString = output.ToString();
            Assert.IsTrue(outputString.Contains("Converted amount: 196"));
        }

        //TC1 amount = -5
        [Test]
        public void PlaceDepositTest1()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("-5\n1\n");
            Console.SetIn(input);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.PlaceDeposit());
            Assert.That(ex.Message, Is.EqualTo("Amount needs to be a number between 0 and 2000. Please try again."));
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit));
        }

        //TC2 amount = 2001
        [Test]
        public void PlaceDepositTest2()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("2001\n1\n");
            Console.SetIn(input);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.PlaceDeposit());
            Assert.That(ex.Message, Is.EqualTo("Amount needs to be a number between 0 and 2000. Please try again."));
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit));
        }

        //TC3 amount = 43
        [Test]
        public void PlaceDepositTest3()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("43\n1\n");
            Console.SetIn(input);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.PlaceDeposit());
            Assert.That(ex.Message, Is.EqualTo("Enter deposit amount in multiples of 5 or 10. Please try again."));
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit));
        }

        //TC4 amount = 0
        [Test]
        public void PlaceDepositTest4()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("0\n1\n");
            Console.SetIn(input);
            var ex = Assert.Throws<System.ArgumentException>(() => Program.PlaceDeposit());
            Assert.That(ex.Message, Is.EqualTo("Amount needs to be a number between 0 and 2000. Please try again."));
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit));
        }

        //TC5 amount = 500
        [Test]
        public void PlaceDepositTest5()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("500\n1\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.PlaceDeposit();
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit + 500));
        }

        //TC6 amount = 500, cancel deposit
        [Test]
        public void PlaceDepositTest6()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("500\n5\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.PlaceDeposit();
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit));
        }

        //Data driven testing
        private static IEnumerable WithdrawalTestCases
        {
            get
            {
                using (var reader = new StreamReader("TestData.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<WithdrawalTestData>().ToList();
                    foreach (var record in records)
                    {
                        yield return new TestCaseData(record.WithdrawalAmount, record.ExpectedResult);
                    }
                }
            }
        }

        [Test, TestCaseSource(nameof(WithdrawalTestCases))]
        public void MakeWithdrawalTest(int withdrawalAmount, bool expectedResult)
        {
            Program.InitializeData();
            Program.selectedUser = Program.userList[0];

            bool result = false;
            try
            {
                Program.MakeWithdrawal(withdrawalAmount);
                result = true;
            }
            catch (Exception) { }

            Assert.AreEqual(expectedResult, result);
        }

        // Class for reading csv file
        private class WithdrawalTestData
        {
            public int WithdrawalAmount { get; set; }
            public bool ExpectedResult { get; set; }
        }

        // Test data source for internal transfer testing from XML
        private static IEnumerable InternalTransferTestCasesFromXml
        {
            get
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load("TestData.xml");

                foreach (XmlNode testCaseNode in xmlDoc.DocumentElement.ChildNodes)
                {
                    decimal transferAmount = decimal.Parse(testCaseNode.SelectSingleNode("TransferAmount").InnerText);
                    int recipientAccountNumber = int.Parse(testCaseNode.SelectSingleNode("RecipientAccountNumber").InnerText);
                    string recipientAccountName = testCaseNode.SelectSingleNode("RecipientAccountName").InnerText;
                    bool expectedResult = bool.Parse(testCaseNode.SelectSingleNode("ExpectedResult").InnerText);

                    yield return new TestCaseData(transferAmount, recipientAccountNumber, recipientAccountName, expectedResult);
                }
            }
        }

        [Test, TestCaseSource(nameof(InternalTransferTestCasesFromXml))]
        public void InternalTransferFromXmlTest(decimal transferAmount, int recipientAccountNumber, string recipientAccountName, bool expectedResult)
        {
            Program.selectedUser = Program.userList[0];
            bool result = false;
            try
            {
                var internalTransfer = new InternalTransferTransaction
                {
                    Amount = transferAmount,
                    RecipientAccountNumber = recipientAccountNumber,
                    RecipientAccountName = recipientAccountName
                };
                Program.ProcessInternalTransfer(internalTransfer);
                result = true;
            }
            catch (Exception) { }
            Assert.AreEqual(expectedResult, result);
        }




        //White Box testing for method MakeWithdrawal

        // Test Case 1: Test successful withdrawal with console input 
        // Amount = 30, Balance = 1000
        [Test]
        public void MakeWithdrawalFromConsole_SuccessfulWithdrawal_ReturnsTrue()
        {
            decimal initialBalance = 1000.00m;
            Program.selectedUser = new User { Id = 7, AccountBalance = initialBalance };
            var input = new StringReader("0\n30\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.MakeWithdrawal();
            decimal expectedBalance = initialBalance - 30;
            Assert.AreEqual(expectedBalance, Program.selectedUser.AccountBalance);
            Assert.That(output.ToString().Contains("You have successfully withdrawn 30"));
        }

        // Test Case 2: Test successful withdrawal with first wrong input choice 
        // Amount = 50, Balance = 1000
        [Test]
        public void MakeWithdrawalFirstWrongChoice_SuccessfulWithdrawal_ReturnsTrue()
        {
            decimal initialBalance = 1000.00m;
            Program.selectedUser = new User { Id = 7, AccountBalance = initialBalance };
            var input = new StringReader("9\n\n3\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            Program.MakeWithdrawal();
            decimal expectedBalance = initialBalance - 50;
            Assert.AreEqual(expectedBalance, Program.selectedUser.AccountBalance);
            Assert.That(output.ToString().Contains("Invalid input. Try again."));
            Assert.That(output.ToString().Contains("You have successfully withdrawn 50"));
        }

        // Test Case 3: Test withdrawal with invalid amount (negative)
        // Amount = -10, Balance = 500
        [Test]
        public void MakeWithdrawal_InvalidAmountNegative_ThrowsException()
        {
            decimal initialBalance = 500.00m;
            Program.selectedUser = new User { Id = 7, AccountBalance = initialBalance };
            var exception = Assert.Throws<ArgumentException>(() => Program.MakeWithdrawal(-10));
            Assert.That(exception.Message, Is.EqualTo("Amount needs to be greater than zero. Try again"));
            Assert.AreEqual(initialBalance, Program.selectedUser.AccountBalance);
        }

        // Test Case 4: Test withdrawal with invalid amount (not a multiple of 5)
        // Amount = 33, Balance = 500
        [Test]
        public void MakeWithdrawal_InvalidAmountNotMultipleOfFive_ThrowsException()
        {
            decimal initialBalance = 500.00m;
            Program.selectedUser = new User { Id = 7, AccountBalance = initialBalance };
            var exception = Assert.Throws<ArgumentException>(() => Program.MakeWithdrawal(33));
            Assert.That(exception.Message, Is.EqualTo("You can only withdraw amount in multiples of 5 or 10 euros. Please try again."));
            Assert.AreEqual(initialBalance, Program.selectedUser.AccountBalance);
        }

        // Test Case 5: Test withdrawal with insufficient balance
        // Amount = 1000, Balance = 500
        [Test]
        public void MakeWithdrawal_InsufficientBalance_ThrowsException()
        {
            decimal initialBalance = 500.00m;
            Program.selectedUser = new User { Id = 7, AccountBalance = initialBalance };
            var exception = Assert.Throws<ArgumentException>(() => Program.MakeWithdrawal(1000));
            Assert.That(exception.Message, Is.EqualTo("Withdrawal failed. Your balance is too low to withdraw 1000"));
            Assert.AreEqual(initialBalance, Program.selectedUser.AccountBalance);
        }

        // Test Case 6: Test successful withdrawal with parameter sent
        // Amount = 50, Balance = 500
        [Test]
        public void MakeWithdrawalParameterSent_SuccessfulWithdrawal_ReturnsTrue()
        {
            decimal initialBalance = 500.00m;
            Program.selectedUser = new User { Id = 7, AccountBalance = initialBalance };
            Program.MakeWithdrawal(50);
            decimal expectedBalance = initialBalance - 50;
            Assert.AreEqual(expectedBalance, Program.selectedUser.AccountBalance);
        }

        //White Box testing for method ProcessInternalTransfer

        // Test Case 1: Test transfer with invalid amount (negative)
        // Amount = -50
        [Test]
        public void InternalTransfer_InsufficientBalance_ThrowsExcepti()
        {
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(-50, "Bakir Pljakic", 111222);
            Assert.Throws<ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
        }

        // Test Case 2: Test transfer with insufficient balance
        // Amount = 700, Balance = 200
        [Test]
        public void InternalTransfer_InsufficientBalance_ThrowsException()
        {
            Program.selectedUser = Program.userList[1];
            Program.selectedUser.AccountBalance = 200;
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(700, "Bakir Pljakic", 111222);
            Assert.Throws<ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
        }

        // Test Case 3: Test transfer with invalid reciever bank account
        // RecipientAccountNumber = 0
        [Test]
        public void InternalTransferException_InvalidBankAccount_ThrowsException()
        {
            Program.selectedUser = Program.userList[0];
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Bakir Pljakic", 0);
            Assert.Throws<ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
        }

        // Test Case 4: Test transfer with non-existing recipient name
        // RecipientAccountName = Invalid name
        [Test]
        public void InternalTransferExceptionTest_InvalidName_ThrowsException() 
        {
            Program.selectedUser = Program.userList[0];
            InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Invalid name", 112233);
            Assert.Throws<ArgumentException>(() => Program.ProcessInternalTransfer(internalTransferTransaction));
        }

        // Test Case 5: Test successful transfer
        // Amount = 500, Balance = 2000
        
        [Test]

        public void InternalTransferTest_SuccessfulTransfer()
        {
            {
                Program.selectedUser = Program.userList[1];
                Program.selectedUser.AccountBalance = 2000;
                InternalTransferTransaction internalTransferTransaction = new InternalTransferTransaction(500, "Kerim Halilovic", 112233);
                Program.ProcessInternalTransfer(internalTransferTransaction);
                Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(1500));
            }
        }
        
    }
}