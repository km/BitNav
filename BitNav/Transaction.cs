﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        private long creationTime;
        private long receivedTime;
        private long confirmationTime;
        private WebClient wc;
        public Transaction(object coin, int amount, string address, string senderAddress = null)
        {
            this.coin = coin;
            this.amount = amount;
            this.address = address;
            this.senderAddress = senderAddress;
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
                            if (checkEqualWithMargin(amount, txAmounts.GetProperty("value").GetInt32(), 0))
                            {
                                transactionHash = tx.GetProperty("tx_hash").GetString();
                                receivedTime = unixTime;
                                //checks if confirmation time exists and parses it if it does
                                try
                                {
                                    string confirmedString = tx.GetProperty("confirmed").GetString();
                                    DateTime confirmeDateTime = DateTime.ParseExact(confirmedString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                                    this.confirmationTime = ((DateTimeOffset)confirmeDateTime).ToUnixTimeSeconds();
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



        private bool checkEqualWithMargin(int a, int b, int margin)
        {
            return a >= b - margin && a <= b + margin;
        }
        //returns null if transaction is not received or the transaction hash if it is
        public string getTransactionHash()
        {
            return transactionHash;
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