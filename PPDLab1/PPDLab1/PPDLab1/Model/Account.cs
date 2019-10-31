using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PPDLab1.Model
{
    public class Account : Loggable
    {
        public uint AccountId { get; set; }

        public int Balance { get; set; }

        private static readonly object accountMutex = new object();

        public Account(uint id)
        {
            AccountId = id;
            Balance = 0;
            TransactionLog = new List<TransactionLogItem>();
        }

        public Account(uint id, int balance)
        {
            AccountId = id;
            Balance = balance;
            TransactionLog = new List<TransactionLogItem>();
        }

        public bool Withdraw(int sum)
        {
            lock(accountMutex)
            {
                if (Balance < sum)
                    return false;

                Balance -= sum;
                return true;
            }
        }

        public bool Deposit(int sum)
        {
            lock(accountMutex)
            {
                Balance += sum;
                return true;
            }
        }

        
    }
}
