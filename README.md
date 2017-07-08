# Borlay.Iota.Library
.netStandard 1.4 library for IOTA wallets.



## Getting started

Get address with balance

```cs
var api = new IotaApi("http://node.iotawallet.info:14265", 15);
var address = await api.GetAddress("YOURSEED", 0);

// use address
var balance = address.Balance;
var hasTransactions = address.HasTransactions;

// you can renew yous address balance
api.RenewBalances(address);
// or
api.RenewAddresses(address);
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

## Nuget

```PowerShell
Install-Package Borlay.Iota.Library
```

Related project here: https://github.com/iotaledger/iota.lib.csharp
