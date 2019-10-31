using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPDLab1.Exceptions;
using PPDLab1.Model;

namespace PPDLab1Tests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        [ExpectedException(typeof(TransactionException))]
        public void PerformTransactionNegativeSumExceptionIsThrown()
        {
            Bank bank = new Bank();

            Account account1 = new Account(0, 500);
            Account account2 = new Account(1, 200);

            bank.PerformTransaction(account1, account2, -100);
        }

        [TestMethod]
        public void PerformNormalTransactionBalancesCorrect()
        {
            Bank bank = new Bank();

            Account account1 = new Account(0, 500);
            Account account2 = new Account(1, 200);

            bank.PerformTransaction(account1, account2, 200);

            Assert.IsTrue(account1.Balance == 300);
            Assert.IsTrue(account2.Balance == 400);

            bank.PerformTransaction(account2, account1, 400);
            Assert.IsTrue(account1.Balance == 700);
            Assert.IsTrue(account2.Balance == 0);
        }

        [TestMethod]
        public void PerformNormalTransactionLogsAdded()
        {
            Bank bank = new Bank();

            Account account1 = new Account(0, 500);
            Account account2 = new Account(1, 200);

            bank.PerformTransaction(account1, account2, 100);

            Assert.IsTrue(account1.TransactionLog[0].SourceAccountId == account1.AccountId);
            Assert.IsTrue(account1.TransactionLog[0].DestinationAccountId == account2.AccountId);
            Assert.IsTrue(account1.TransactionLog[0].TransactionSum == 100);

            Assert.IsTrue(account2.TransactionLog[0].SourceAccountId == account1.AccountId);
            Assert.IsTrue(account2.TransactionLog[0].DestinationAccountId == account2.AccountId);
            Assert.IsTrue(account2.TransactionLog[0].TransactionSum == 100);
        }

        [TestMethod]
        public void PerformNormalTransactionLogIdsCorrect()
        {
            uint transactionId = 0;
            Bank bank = new Bank();

            Account account1 = new Account(0, 500);
            Account account2 = new Account(1, 200);

            bank.PerformTransaction(account1, account2, 100);

            Assert.IsTrue(account1.TransactionLog[0].TransactionId == transactionId);
            Assert.IsTrue(account2.TransactionLog[0].TransactionId == transactionId);

            bank.PerformTransaction(account1, account2, 50);
            transactionId++;

            Assert.IsTrue(account1.TransactionLog[1].TransactionId == transactionId);
            Assert.IsTrue(account2.TransactionLog[1].TransactionId == transactionId);
        }

        [TestMethod]
        public void PerformTransactionNotEnoughBalanceTransactionFails()
        {
            Bank bank = new Bank();

            Account account1 = new Account(0, 500);
            Account account2 = new Account(1, 200);

            Assert.IsFalse(bank.PerformTransaction(account1, account2, 600));
        }

        [TestMethod]
        [ExpectedException(typeof(TransactionException))]
        public void PerformTransactionSameAccountExceptionThrown()
        {
            Bank bank = new Bank();

            Account account1 = new Account(0, 500);

            bank.PerformTransaction(account1, account1, 500);
        }
    }
}