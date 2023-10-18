using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitNav
{
    public class Transaction
    {
        private object coin;
        private int amount;
        private string address;
        private string senderAddress;
        private string transactionHash;

        public Transaction(object coin, int amount, string address, string senderAddress = null)
        {
            this.coin = coin;
            this.amount = amount;
            this.address = address;
            this.senderAddress = senderAddress;
            this.transactionHash = null;
        }

        //returns true if transaction is received
        public bool checkTransaction()
        {
            return true;
        }

        //returns null if transaction is not received or the transaction hash if it is
        public string getTransactionHash()
        {
            return this.transactionHash;
        }

        public string getSenderAddress()
        {
            return senderAddress;
        }

        public string getReceiverAddress()
        {
            return address;
        }
    }
}
