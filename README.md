
# BitNav - Cryptocurrency Payment Library

BitNav is a .NET 6 library for handling and managing cryptocurrency payments, initially focusing on Bitcoin. This library provides a set of classes and methods to facilitate the creation, monitoring, and confirmation of transactions.

## Table of Contents

-   [Installation](#installation)
-   [Usage](#usage)
    -   [Bitcoin Class](#bitcoin-class)
    -   [Transaction Class](#transaction-class)
    -   [TransactionManager Class](#transactionManager-class)
- [Events](#events) 
	- [TransactionReceived Event](#transactionreceived-event) 
	- [TransactionConfirmed Event](#transactionconfirmed-event)
-   [Examples](#examples)


----------

## Installation

To use BitNav in your C# project, follow these steps:

1.  Download the latest release from the [GitHub repository](https://github.com/km/BitNav).
2. Install the dependency [NBitcoin](https://github.com/MetacoSA/NBitcoin)
3.  Add the downloaded DLL file to your project's references.
4.  Import the necessary namespaces:
```csharp
using BitNav;
```

## Usage

### Bitcoin Class

The `Bitcoin` class provides functionalities related to Bitcoin transactions and addresses.

#### Constructor
```csharp
public Bitcoin(string privateKey, string givenAddress = null)
```
-   `privateKey`: The private key associated with the Bitcoin address.
-   `givenAddress`: (Optional) A specific Bitcoin address. If not provided, a new address will be generated using the private key.

#### Methods

-   `string GetAddress()`: Returns the Bitcoin address associated with the instance.
    
-   `static long ConvertBitcoinToSatoshis(double bitcoinAmount)`: Converts a Bitcoin amount to satoshis.
### Transaction Class

The `Transaction` class handles cryptocurrency transactions.

#### Constructor
```csharp
public Transaction(object coin, double amount, string address)
```
-   `coin`: The cryptocurrency object (e.g., Bitcoin) associated with the transaction.
-   `amount`: The amount of cryptocurrency to be received.
-   `address`: Address to receive the cryptocurrency.

#### Methods

-   `bool checkTransactionReceived()`: Checks if the transaction has been received.
    
-   `bool checkTransactionConfirmed()`: Checks if the transaction has been confirmed.
    
-   `string getTransactionHash()`: Returns the transaction hash.
    
-   `string getReceiverAddress()`: Returns the recipient's address.
    
-   `long getCreationTime()`: Returns the transaction's creation time in Unix timestamp.
    
-   `long getReceivedTime()`: Returns the time when the transaction was received in Unix timestamp.
    
-   `long getConfirmationTime()`: Returns the time when the transaction was confirmed in Unix timestamp.
    
-   `double getAmount()`: Returns the amount of cryptocurrency involved in the transaction.
    
-   `object getCoin()`: Returns the cryptocurrency object associated with the transaction.

### TransactionManager Class

The `TransactionManager` class manages a list of transactions, and will check if they're received or confirmed every 2 minutes in the background.

#### Constructor
```csharp
public TransactionManager()
```
#### Methods

-   `Transaction CreateTransaction(object coin, double amount, string address)`: Creates a new transaction.
    
-   `void AddTransaction(Transaction transaction)`: Adds an existing transaction to the manager.
    
-   `void RemoveTransaction(Transaction transaction)`: Removes a transaction from the manager.
    
-   `List<Transaction> GetTransactions()`: Returns the list of all transactions.

## Events
Each instance of the `Transaction.cs` object is equipped with both `TransactionReceived` and `TransactionConfirmed` events, which can be subscribed to. These events are automatically triggered when a `TransactionManager` is active in the background, and the corresponding `Transaction` is managed within it.
### TransactionReceived Event

The `TransactionReceived` event is triggered when the transaction is received to the address.

#### Syntax

```csharp
public event EventHandler<TransactionEventArgs> TransactionReceived;
```
#### Usage
```csharp
// Subscribe to the TransactionReceived event
transaction.TransactionReceived += HandleTransactionReceived;

// Event handler method
private void HandleTransactionReceived(object sender, TransactionEventArgs e)
{
    Transaction receivedTransaction = e.Transaction;
    // Add custom logic to handle the received transaction
}
```
### TransactionConfirmed Event

The `TransactionConfirmed` event is triggered when the transaction is confirmed on the blockchain.

#### Syntax
```csharp
public event EventHandler<TransactionEventArgs> TransactionConfirmed;
```
#### Usage
```csharp
// Subscribe to the TransactionConfirmed event
transaction.TransactionConfirmed += HandleTransactionConfirmed;

// Event handler method
private void HandleTransactionConfirmed(object sender, TransactionEventArgs e)
{
    Transaction confirmedTransaction = e.Transaction;
    // Add custom logic to handle the confirmed transaction
}
```

## Examples

Here's an example of how to use BitNav to create and manage a Bitcoin transaction:
```csharp
// Initialize the Bitcoin object with a private key
Bitcoin bitcoin = new Bitcoin("your_private_key");

// Generate a new Bitcoin address
string recipientAddress = bitcoin.GetAddress();

// Create a transaction manager
TransactionManager manager = new TransactionManager();

// Create a new Bitcoin transaction
Transaction transaction = manager.CreateTransaction(bitcoin, 0.1, recipientAddress);

// Check if the transaction has been received
bool received = transaction.checkTransactionReceived();

if (received)
{
    // Transaction has been received, check if it's confirmed
    bool confirmed = transaction.checkTransactionConfirmed();

    if (confirmed)
    {
        Console.WriteLine("Transaction confirmed!");
    }
    else
    {
        Console.WriteLine("Transaction received but not yet confirmed.");
    }
}
else
{
    Console.WriteLine("Transaction not yet received.");
}
```

---

## Acknowledgements

This project is built using the [NBitcoin](https://github.com/MetacoSA/NBitcoin) library. Additionally, it utilizes the [BlockCypher](https://www.blockcypher.com/) API for blockchain interaction.

---

*Note: NBitcoin is a comprehensive Bitcoin library for the .NET platform, and BlockCypher provides a range of blockchain services and APIs.*
