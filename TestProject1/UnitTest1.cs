using ATMConsoleApp.Models;
using ATMConsoleApp;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.IO;
using System;
using NUnit.Framework.Constraints;

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
    }
}