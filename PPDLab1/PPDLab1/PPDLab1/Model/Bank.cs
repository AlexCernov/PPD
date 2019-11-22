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


        // This method uses a lock on the account
        //  Creates a transaction (initial outcome state = false) then it does the transfer using the methods withdraw and deposit(which locks the account)
        public bool PerformTransactionAccountLock(Account sourceAccount, Account destinationAccount, int sum)
        {
            /*
            if (TransactionIdCounter % 100000 == 0)
            {
                lock (this)
                {
                    PerformeChecks();
                }
            }*/
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

            bool transferFinished = TransferFunds(sourceAccount, destinationAccount, sum);
            if (transferFinished == false)
            {
                this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);
                lock (this)
                {
                    TransactionIdCounter++;
                }
                return false;
            }
            else
            {
                transaction.Outcome = true;
                this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);

                lock (this)
                {
                    TransactionIdCounter++;
                }

                return true;
            }
        }
        //this method uses a global lock 
        // Creates a transaction (initial outcome state = false) then locks the bank to perform a transfer (accounts are not locked)
        public bool PerformTransactionBankLock(Account sourceAccount, Account destinationAccount, int sum)
        {
            bool transferFinished = false;
            /*if (TransactionIdCounter % 100000 == 0)
            {
                lock (this)
                {
                    PerformeChecks();
                }
            }*/
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
                this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);
                lock (this)
                {
                    TransactionIdCounter++;
                }
                return false;
            }
            else
            {
                transaction.Outcome = true;
                this.AddToAllLogs(transaction, this, sourceAccount, destinationAccount);

                lock (this)
                {
                    TransactionIdCounter++;
                }

                return true;
            }
        }


        // method used to transfer funds thread safe(with a lock on account)
        private bool TransferFunds(Account sourceAccount, Account destinationAccount, int sum)
        {
            if (sourceAccount.Withdraw(sum))
            {
                destinationAccount.Deposit(sum);
                return true;
            }
            return false;
        }
        

        // this method is used to transfer funds thread unsafe ( used for global lock ) -> no account locking
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

                // decide whether this is the source or the destination account
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

        public void PerformeChecks()
        {
            lock (this)
            {
                Console.WriteLine("Starting account checks...");
                int balanceFailCounter = 0;
                int logFailCounter = 0;
                foreach (var account in this.BankAccounts.Values)
                {
                    bool balanceCheckResult = this.CheckAccountTransactions(account.AccountId);
                    bool logCheckResult = this.CheckAccountTransactionLogs(account.AccountId);

                    balanceFailCounter += balanceCheckResult ? 0 : 1;
                    logFailCounter += logCheckResult ? 0 : 1;
                    string balanceCheckString = balanceCheckResult ? "CORRECT" : "FAILED";
                    string logCheckString = logCheckResult ? "CORRECT" : "FAILED";

                    Console.WriteLine("ACCOUNT " + account.AccountId + " BALANCE CHECK: " + balanceCheckString);
                    //Console.WriteLine("ACCOUNT " + account.AccountId + "     LOG CHECK: " + logCheckString);
                }

                Console.WriteLine("BALANCE FAIL COUNT: " + balanceFailCounter);
                Console.WriteLine("LOG FAIL COUNT: " + logFailCounter);
            }
            
        }
    }
}
