using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPDLab1.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPDLab1Tests
{
    [TestClass]
    public class BankTests
    {
        [TestMethod]
        public void AddBankAccountSuccessful()
        {
            Bank bank = new Bank();
            Account account1 = new Account(1, 0);

            Assert.IsTrue(bank.AddAccount(account1));
        }

        [TestMethod]
        public void AddBankAccountDuplicateIdFails()
        {
            Bank bank = new Bank();

            Account account1 = new Account(1, 100);
            Account account2 = new Account(1, 200);

            Assert.IsTrue(bank.AddAccount(account1));
            Assert.IsFalse(bank.AddAccount(account2));
        }

        public void AddBankAccountIdIsZeroFails()
        {
            Bank bank = new Bank();
            Account account1 = new Account(0, 100);

            Assert.IsFalse(bank.AddAccount(account1));
        }
    }
}
