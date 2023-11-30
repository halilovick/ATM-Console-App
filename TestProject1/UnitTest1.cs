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

namespace TestProject1
{
    public class Tests
    {
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

        [Test]
        public void DepositMoneyTest()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            Program.PlaceDeposit(100);
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit + 100));
        }

        [Test]
        public void DepositMoneyConsoleTest()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("100\n1");
            Console.SetIn(input);
            Program.PlaceDeposit();
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit + 100));
        }

        [Test]
        public void DepositMoneyCancelTest()
        {
            Program.selectedUser = Program.userList[0];
            var balanceBeforeDeposit = Program.selectedUser.AccountBalance;
            var input = new StringReader("100\n5");
            Console.SetIn(input);
            Program.PlaceDeposit();
            Assert.That(Program.selectedUser.AccountBalance, Is.EqualTo(balanceBeforeDeposit));
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
            var ex = Assert.Throws<System.ArgumentException>( () => Program.MakeWithdrawal(249));
            Assert.That(ex.Message, Is.EqualTo("You can only withdraw amount in multiples of 5 or 10 euros. Please try again."));
        }


        [Test]
        public void PlaceDepositExceptionTest()
        {
            Program.selectedUser = Program.userList[0];
            var ex = Assert.Throws<System.ArgumentException>(() => Program.PlaceDeposit(2001));
            Assert.That(ex.Message, Is.EqualTo("Amount needs to be a number between 0 and 2000. Please try again."));
        }


        [Test]
        public void PlaceDepositExceptionTest2()
        {
            Program.selectedUser= Program.userList[0];
            var ex = Assert.Throws<System.ArgumentException>(() => Program.PlaceDeposit(139));
            Assert.That(ex.Message, Is.EqualTo("Enter deposit amount in multiples of 5 or 10. Please try again."));
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

        /*[Test]
        public void CheckUserCredentialsTest()
        {
            Program.selectedUser = Program.userList[0];
            var input1 = new StringReader("111222");
            Console.SetIn(input1);
            var input2 = new StringReader("1234");
            Console.SetIn(input2);
            var provjera = Program.CheckUserCredentials();
            Assert.IsTrue(provjera);
        }*/ // besk petlja

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

        /*[Test]
        public void UserLoginTest()
        {
            var input = new StringReader("111222\n1234\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);
            var user = ATMScreen.UserLogin();
            Assert.That(user, Is.EqualTo(Program.userList[0]));
        }*/

        [Test]
        public void CreateInternalTransferTest()
        {
            var input = new StringReader("112233\n100\nKerim Halilovic");
            Console.SetIn(input);
            ATMScreen screen = new ATMScreen();
            var internalTransferTransaction = screen.CreateInternalTransferTransaction();
            Assert.That(internalTransferTransaction.RecipientAccountNumber, Is.EqualTo(112233));
        }

        /*[Test]
        public void InvalidInputConvertTest()
        {
            Utility.Convert<int>("InvalidPrompt");
            var output = new StringWriter();
            Console.SetOut(output);
            Assert.That(output.ToString(), Is.EqualTo("Invalid input. Try again."));
        }*/ // popravit

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

        /*[Test]
        public void GetPINInputTest()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var input = new StringReader("1\n2\n3\n4\n");
            Console.SetIn(input);
            var pin = Utility.GetPINInput("Enter your card PIN");
            Assert.That(pin, Is.EqualTo("1234"));
        }*/
    }
}