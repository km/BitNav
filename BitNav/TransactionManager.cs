using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BitNav
{
    public class TransactionManager
    {
        private List<Transaction> transactions;
        private List<Transaction> confirmedTransactions;


        public TransactionManager()
        {
            
        }

        //create Transaction
        public Transaction CreateTransaction(object coin, int amount, string address)
        {
            Transaction transaction = new Transaction(coin, amount, address);
            transactions.Add(transaction);
            return transaction;
        }
    }
}
