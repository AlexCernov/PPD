using PPDLab1.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PPDLab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Bank bank = new Bank();

            int numAccounts = 10000;
            int numTransactions = 10000000;
            Random random = new Random();

            int numThreads = 16;

            for(int i = 0; i < numAccounts; ++i)
            {
                Account a = new Account((uint)i + 1, 0);
                bank.GiveMoney(a, random.Next(100, 500));
                bank.AddAccount(a);
            }

            List<Thread> threads = new List<Thread>();
            Stopwatch s = new Stopwatch();
            for (int i = 0; i < numThreads; ++i)
            {
                Thread thread = new Thread(() => PerformTransactions(bank, numTransactions / numThreads));
                threads.Add(thread);
                thread.Start();
            }
            s.Start();

            // creating a thread for performing a check in between the transactions
            var threadCheck = new Thread(() => bank.PerformeChecks());
            threadCheck.Start();
            threads.Add(threadCheck);
            foreach (var item in threads)
            {
                item.Join();
            }

            s.Stop();
            Console.WriteLine("Stopwatch Stopped: " + s.ElapsedMilliseconds + " milliseconds");

            /*
            Console.WriteLine("Starting account checks...");
            int balanceFailCounter = 0;
            int logFailCounter = 0;
            foreach (var account in bank.BankAccounts.Values)
            {
                bool balanceCheckResult = bank.CheckAccountTransactions(account.AccountId);
                bool logCheckResult = bank.CheckAccountTransactionLogs(account.AccountId);

                balanceFailCounter += balanceCheckResult ? 0 : 1;
                logFailCounter += logCheckResult ? 0 : 1;
                string balanceCheckString = balanceCheckResult ? "CORRECT" : "FAILED";
                string logCheckString = logCheckResult ? "CORRECT" : "FAILED";

                Console.WriteLine("ACCOUNT " + account.AccountId + " BALANCE CHECK: " + balanceCheckString);
                //Console.WriteLine("ACCOUNT " + account.AccountId + "     LOG CHECK: " + logCheckString);
            }

            Console.WriteLine("BALANCE FAIL COUNT: " + balanceFailCounter);
            Console.WriteLine("LOG FAIL COUNT: " + logFailCounter);*/
            Console.Read();

        }

        public static void PerformTransactions(Bank bank, int numTransactions)
        {
            Random random = new Random();
            
            for(int i = 0; i < numTransactions; ++i)
            {
                uint firstAccountId = (uint)random.Next(1, bank.BankAccounts.Count + 1);

                uint secondAccountId = (uint)random.Next(1, bank.BankAccounts.Count + 1);
                while (secondAccountId == firstAccountId)
                    secondAccountId = (uint)random.Next(1, bank.BankAccounts.Count + 1);

                Account firstAccount = bank.BankAccounts[firstAccountId];
                Account secondAccount = bank.BankAccounts[secondAccountId];

                bank.PerformTransactionBankLock(firstAccount, secondAccount, random.Next(1, 200));
            }
        }
    }
}
