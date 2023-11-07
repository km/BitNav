using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Transactions;

namespace BitNav
{
    public class Transaction
    {
        private object coin;
        private double amount;
        private string address;
        private string transactionHash;
        private long creationTime;
        private long receivedTime;
        private long confirmationTime;
        private WebClient wc;
        public event EventHandler<TransactionEventArgs> TransactionReceived;
        public event EventHandler<TransactionEventArgs> TransactionConfirmed;

        public Transaction(object coin, double amount, string address)
        {
            this.coin = coin;
            this.amount = amount;
            this.address = address;
            this.transactionHash = null;
            this.wc = new WebClient();
            this.creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        //returns true if transaction is received and sets the transaction hash
        public bool checkTransactionReceived()
        {
            if (transactionHash != null)
            {
                return true;
            }
            string url = "";
            switch(coin)
            {
                case Bitcoin: 
                    url = "https://api.blockcypher.com/v1/btc/main/addrs/"+address+"/full";
                    break;
                default:
                    throw new Exception("Invalid coin type");
                    break;
            }

            string json = wc.DownloadString(url);
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            JsonElement txs = root.GetProperty("txrefs");

            foreach (JsonElement tx in txs.EnumerateArray())
            {

                string dateString = tx.GetProperty("received").GetString();
                DateTime dateTime = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                long unixTime = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
                if (unixTime >= creationTime)
                {
                    JsonElement txAmount = tx.GetProperty("outputs");
                    foreach (JsonElement txAmounts in txAmount.EnumerateArray())
                    {
                        Console.WriteLine(txAmounts.GetProperty("addresses").GetString());
                        if (txAmounts.GetProperty("addresses").GetString() == address)
                        {
                            if (checkEqualWithMargin(Bitcoin.ConvertBitcoinToSatoshis(amount), txAmounts.GetProperty("value").GetInt32(), 0))
                            {
                                transactionHash = tx.GetProperty("tx_hash").GetString();
                                receivedTime = unixTime;
                                OnTransactionReceived();
                                //checks if confirmation time exists and parses it if it does
                                try
                                {
                                    string confirmedString = tx.GetProperty("confirmed").GetString();
                                    DateTime confirmeDateTime = DateTime.ParseExact(confirmedString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                                    this.confirmationTime = ((DateTimeOffset)confirmeDateTime).ToUnixTimeSeconds();
                                    OnTransactionConfirmed();
                                }
                                catch
                                {
                                }
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }


            return false;
        }

        public bool checkTransactionConfirmed()
        {
            if (confirmationTime != 0)
            {
                return true;
            }
            else if (transactionHash == null)
            { 
                checkTransactionReceived();

                return confirmationTime != 0;
            }
            string url = "";
            switch (coin)
            {
                case Bitcoin:
                    url = "https://api.blockcypher.com/v1/btc/main/txs/" + transactionHash;
                    break;
                default:
                    throw new Exception("Invalid coin type");
                    break;
            }

            string json = wc.DownloadString(url);
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            try
            {
                string confirmedString = root.GetProperty("confirmed").GetString();
                DateTime confirmeDateTime = DateTime.ParseExact(confirmedString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                this.confirmationTime = ((DateTimeOffset)confirmeDateTime).ToUnixTimeSeconds();
                OnTransactionConfirmed();
                return true;
            }
            catch (Exception e)
            {
            }

            return false;
        }

        private bool checkEqualWithMargin(long a, long b, int margin)
        {
            return a >= b - margin && a <= b + margin;
        }
        //returns null if transaction is not received or the transaction hash if it is
        public string getTransactionHash()
        {
            return transactionHash;
        }

        public string getReceiverAddress()
        {
            return address;
        }

        public long getCreationTime()
        {
            return creationTime;
        }

        public long getReceivedTime()
        {
            return receivedTime;
        }

        public long getConfirmationTime()
        {
            return confirmationTime;
        }
        public double getAmount()
        {
            return amount;
        }
        public object getCoin()
        {
            return coin;
        }

        private void OnTransactionReceived()
        {
            TransactionReceived?.Invoke(this, new TransactionEventArgs(this));
        }
        private void OnTransactionConfirmed()
        {
            TransactionConfirmed?.Invoke(this, new TransactionEventArgs(this));
        }

    }
}
