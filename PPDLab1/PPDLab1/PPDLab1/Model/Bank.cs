using PPDLab1.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPDLab1.Model
{
    public class Bank : Loggable
    {
        private uint TransactionIdCounter { get; set; }

        public Dictionary<uint, Account> BankAccounts { get; set; }

        public Bank()
        {
            BankAccounts = new Dictionary<uint, Account>();
            TransactionLog = new List<TransactionLogItem>();
        }

        public bool AddAccount(Account account)
        {
            // If an account has an invalid id
            if (account.AccountId == 0)
                return false;

            // If there is already an account with the same id in the bank
            if (BankAccounts.ContainsKey(account.AccountId))
                return false;

            BankAccounts.Add(account.AccountId, account);
            return true;
        }

        public void GiveMoney(Account destinationAccount, int sum)
        {
            destinationAccount.Balance += sum;

            // logging the transaction : sourceId = 0 -> money from the bank

            var transaction = new TransactionLogItem(
                TransactionIdCounter, 0, destinationAccount.AccountId, sum, true);

            this.AddToAllLogs(transaction, this, destinationAccount);
            TransactionIdCounter++;
        }

        public bool PerformTransaction(Account sourceAccount, Account destinationAccount, int sum)
        {
            // First we log that the transaction begin : intial outcome = false

            var transaction = new TransactionLogItem(
                TransactionIdCounter,
                sourceAccount.AccountId,
                destinationAccount.AccountId,
                sum,
                false);


            if (sourceAccount.AccountId == destinationAccount.AccountId)
                throw new TransactionException("Cannot perform a transfer into the same account.");

            if (sum < 0)
                throw new TransactionException("Transaction sum is negative.");

            // If the source account doesn't have enough money for the transfer
            if (sourceAccount.Balance < sum)
            {
                this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);
                TransactionIdCounter++;
                return false;
            }

            sourceAccount.Balance -= sum;
            destinationAccount.Balance += sum;

            transaction.Outcome = true;
            this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);
            TransactionIdCounter++;

            return true;
        }

        public bool PerformTransactionThreadSafe(Account sourceAccount, Account destinationAccount, int sum)
        {
            bool transferFinished = false;

            var transaction = new TransactionLogItem(
                TransactionIdCounter,
                sourceAccount.AccountId,
                destinationAccount.AccountId,
                sum,
                false);

            if (sourceAccount.AccountId == destinationAccount.AccountId)
                throw new TransactionException("Cannot perform a transfer into the same account.");

            if (sum < 0)
                throw new TransactionException("Transaction sum is negative.");

            transferFinished = TransferFunds(sourceAccount, destinationAccount, sum);

            if (transferFinished == false)
            {
                //this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);
                lock (this)
                {
                    TransactionIdCounter++;
                }
                return false;
            }
            else
            {
                transaction.Outcome = true;
                //this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);

                lock (this)
                {
                    TransactionIdCounter++;
                }

                return true;
            }
        }

        public bool PerformTransactionThreadSafeInefficient(Account sourceAccount, Account destinationAccount, int sum)
        {
            bool transferFinished = false;

            var transaction = new TransactionLogItem(
                TransactionIdCounter,
                sourceAccount.AccountId,
                destinationAccount.AccountId,
                sum,
                false);

            if (sourceAccount.AccountId == destinationAccount.AccountId)
                throw new TransactionException("Cannot perform a transfer into the same account.");

            if (sum < 0)
                throw new TransactionException("Transaction sum is negative.");

            lock(this)
            {
                transferFinished = TransferFundsSimple(sourceAccount, destinationAccount, sum);
            }
            
            if (transferFinished == false)
            {
                //this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);
                lock (this)
                {
                    TransactionIdCounter++;
                }
                return false;
            }
            else
            {
                transaction.Outcome = true;
                //this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);

                lock (this)
                {
                    TransactionIdCounter++;
                }

                return true;
            }
        }

        private bool TransferFunds(Account sourceAccount, Account destinationAccount, int sum)
        {
            if (sourceAccount.Withdraw(sum))
            {
                destinationAccount.Deposit(sum);
                return true;
            }
            return false;
        }

        private bool TransferFundsSimple(Account sourceAccount, Account destinationAccount, int sum)
        {
            if (sourceAccount.Balance < sum)
                return false;

            sourceAccount.Balance -= sum;
            destinationAccount.Balance += sum;

            return true;
        }

        private void AddToAllLogs(TransactionLogItem transaction, params Loggable[] logs)
        {
            foreach(var log in logs)
            {
                log.AddToTransactionLog(new TransactionLogItem(
                    transaction.TransactionId,
                    transaction.SourceAccountId,
                    transaction.DestinationAccountId,
                    transaction.TransactionSum,
                    transaction.Outcome));
            }
        }

        public bool CheckAccountTransactions(uint accountId)
        {
            Account accountToCheck = this.BankAccounts[accountId];
            int accountBalance = accountToCheck.Balance;
            int checkBalance = 0;

            foreach(var log in accountToCheck.TransactionLog)
            {
                if (log.Outcome == true)
                {
                    if (log.SourceAccountId == accountId)
                        checkBalance -= log.TransactionSum;
                    else if (log.DestinationAccountId == accountId)
                        checkBalance += log.TransactionSum;
                }
            }

            return accountBalance == checkBalance;
        }

        public bool CheckAccountTransactionLogs(uint accountId)
        {
            Account accountToCheck = this.BankAccounts[accountId];

            foreach(var log in accountToCheck.TransactionLog)
            {
                uint peerId = log.SourceAccountId == accountId ? 
                    log.DestinationAccountId : log.SourceAccountId;

                if(peerId != 0)
                {
                    Account peerAccount = BankAccounts[peerId];
                    bool found = false;
                    foreach (var peerLog in peerAccount.TransactionLog)
                    {
                        if (log == peerLog)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found == false)
                        return false;
                }
            }

            return true;
        }
    }
}
