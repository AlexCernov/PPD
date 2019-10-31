using System;
using System.Collections.Generic;
using System.Text;

namespace PPDLab1.Exceptions
{
    public class TransactionException : Exception
    {
        public TransactionException() : base() { }
        public TransactionException(string message) : base(message) { }
    }
}
