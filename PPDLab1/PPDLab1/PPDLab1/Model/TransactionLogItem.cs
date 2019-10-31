using System;
using System.Collections.Generic;
using System.Text;

namespace PPDLab1.Model
{
    public class TransactionLogItem
    {
        public uint TransactionId { get; set; }

        public uint SourceAccountId { get; set; }

        public uint DestinationAccountId { get; set; }

        public int TransactionSum { get; set; }

        public bool Outcome { get; set; }

        public TransactionLogItem(uint transId, uint srcId, uint destId, int sum, bool outcome)
        {
            TransactionId = transId;
            SourceAccountId = srcId;
            DestinationAccountId = destId;
            TransactionSum = sum;
            Outcome = outcome;
        }

        // Transaction log displayed for a general view
        public string GetTransactionLogString()
        {
            string outcomeText = Outcome ? "PASSED" : "FAILED";

            return 
                "Outcome: " + outcomeText + ' ' +
                "ID: " + TransactionId + ' ' +
                "Source: " + SourceAccountId + ' ' +
                "Destination: " + DestinationAccountId + ' ' +
                "Amount: " + TransactionSum;
        }

        // Transaction log displayed for a certain account
        public string GetTransactionLogString(uint accountId)
        {
            string transactionType = accountId == SourceAccountId ? "Send" : "Receive";
            string partnerType = accountId == SourceAccountId ? "To" : "From";
            string outcomeText = Outcome ? "PASSED" : "FAILED";

            return
                "Outcome: " + outcomeText + ' ' +
                "ID: " + TransactionId + ' ' +
                "Type: " + transactionType + ' ' +
                "Amount: " + TransactionSum + ' ' +
                partnerType + ": " + TransactionSum;
        }

        public static bool operator==(TransactionLogItem item1, TransactionLogItem item2)
        {
            return
                item1.TransactionId == item2.TransactionId &&
                item1.SourceAccountId == item2.SourceAccountId &&
                item1.DestinationAccountId == item2.DestinationAccountId &&
                item1.TransactionSum == item2.TransactionSum &&
                item1.Outcome == item2.Outcome;
        }

        public static bool operator !=(TransactionLogItem item1, TransactionLogItem item2)
        {
            return
                item1.TransactionId != item2.TransactionId ||
                item1.SourceAccountId != item2.SourceAccountId ||
                item1.DestinationAccountId != item2.DestinationAccountId ||
                item1.TransactionSum != item2.TransactionSum ||
                item1.Outcome != item2.Outcome;
        }
    }
}
