using ATMConsoleApp.Models;
using ATMConsoleApp;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

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
    }
}