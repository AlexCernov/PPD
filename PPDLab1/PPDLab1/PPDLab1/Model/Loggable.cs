using System;
using System.Collections.Generic;
using System.Text;

namespace PPDLab1.Model
{
    public class Loggable
    {
        public List<TransactionLogItem> TransactionLog { get; set; }

        public void AddToTransactionLog(TransactionLogItem item)
        {
            lock (TransactionLog)
            {
                TransactionLog.Add(item);
            }
        }
    }
}
