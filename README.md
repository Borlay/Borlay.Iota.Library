# Introduction

The Borlay.Iota.Library implements [[IOTA IRI api calls]](https://github.com/iotaledger/wiki/blob/master/api-proposal.md).
It also can do proof of work for you.

http://iota.org
https://github.com/iotaledger

## Technologies & dependencies

The Borlay.Iota.Library is writen for [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) framework. It can be used in .net 4.5 and .net core 1.0 as well. For full .net frameworks support visit [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)


## Nuget

```PowerShell
Install-Package Borlay.Iota.Library
```

## Getting started

Get address with balance and transactions hashes
```cs
var api = new IotaApi("http://node.iotawallet.info:14265", 15);
var address = await api.GetAddress("YOURSEED", 0);

// use address
var balance = address.Balance;
var transactionHashes = address.Transactions;
```

 Renew your addresses
 ```cs
api.RenewBalances(address); // gets balances
api.RenewTransactions(address); // gets transactions hashes
api.RenewAddresses(address); // both
```

You can send empty transaction simply by doing this
```cs
var transfer = new TransferItem()
{
  Address = "ADDRESS",
  Value = 0,
  Message = "MESSAGETEST",
  Tag = "TAGTEST"
};
var transactionItem = await api.SendTransfer(transfer, CancellationToken.None);
```

Or you can send transaction with value
```cs
var transfer = new TransferItem()
{
  Address = "ADDRESS",
  Value = 1000,
  Message = "MESSAGETEST",
  Tag = "TAGTEST"
};
var transactionItem = await api.SendTransfer(transfer, "YOURSEED", 0, CancellationToken.None);
```

# POW

You can do pow (attachToTangle) like this
```cs
var transactionTrytes = transfer.CreateTransactions().GetTrytes(); // gets transactions from transfer and then trytes
var toApprove = await IriApi.GetTransactionsToApprove(9); // gets transactions to approve
var trunk = toApprove.TrunkTransaction;
var branch = toApprove.BranchTransaction;

var trytesToSend = await transactionTrytes
                .DoPow(trunk, branch, 15, 0, CancellationToken.None); // do pow
await BroadcastAndStore(trytesToSend); // broadcast and send trytes
```
