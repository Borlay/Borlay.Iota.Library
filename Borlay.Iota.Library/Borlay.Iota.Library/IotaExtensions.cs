using Borlay.Iota.Library.Exceptions;
using Borlay.Iota.Library.Models;
using Borlay.Iota.Library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library
{
    public static class IotaExtensions
    {
        public static Task<TransactionItem[]> SendTransfer(this IotaApi api, TransferItem transferItem, CancellationToken cancellationToken)
        {
            var transactionItems = transferItem.CreateTransactions();
            return api.SendTransactions(transactionItems, cancellationToken);
        }

        public static string[] GetTrytes(this IEnumerable<TransactionItem> transactions)
        {
            var transactionTrytes = transactions
                .OrderByDescending(o => o.CurrentIndex)
                .Select(t => t.ToTransactionTrytes())
                .ToArray();
            return transactionTrytes;
        }

        private static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static IEnumerable<TransactionItem> CreateDepositTransaction(TransferItem transferItem)
        {
            var timestamp = IotaApiUtils.CreateTimeStampNow().ToString();

            var tag = transferItem.Tag ?? Constants.EmptyTag;
            // Pad for required 27 tryte length
            while (tag.Length < 27) tag += '9';

            var messages = ChunksUpto(transferItem.Message, 2187).ToArray();
            for (int i = 0; i < messages.Length; i++)
            {
                var message = messages[i];
                while (message.Length < 2187) message += '9';

                var transactionItem = new TransactionItem()
                {
                    Address = transferItem.Address,
                    SignatureFragment = message,
                    Value = (i == 0 ? transferItem.Value : 0).ToString(),
                    Timestamp = timestamp,
                    Tag = tag,
                };

                yield return transactionItem;
            }
        }

        private static IEnumerable<TransactionItem> CreateWithdrawalTransactions(long withdrawAmount, string reminderAddress, params AddressItem[] addressItems)
        {
            if (string.IsNullOrWhiteSpace(reminderAddress))
                throw new ArgumentNullException(nameof(reminderAddress));

            var curl = new Curl();

            foreach (var addressItem in addressItems)
            {
                if (addressItem.Balance <= 0)
                    continue;

                var timestamp = IotaApiUtils.CreateTimeStampNow().ToString();

                var amount = addressItem.Balance;
                withdrawAmount -= amount;

                var transactionItem = new TransactionItem()
                {
                    Address = addressItem.Address,
                    Value = (-amount).ToString(), // withdraw all amount
                    Timestamp = timestamp,
                };
                yield return transactionItem;

                transactionItem = new TransactionItem()
                {
                    Address = addressItem.Address,
                    Value = "0",
                    Timestamp = timestamp,
                };
                yield return transactionItem;

                if (withdrawAmount < 0) // deposit remind amount to reminder address
                {
                    var message = "";
                    while (message.Length < 2187) message += '9';

                    transactionItem = new TransactionItem()
                    {
                        Address = reminderAddress,
                        Value = Math.Abs(withdrawAmount).ToString(),
                        Timestamp = timestamp,
                        SignatureFragment = message
                    };
                    yield return transactionItem;
                }

                if (withdrawAmount <= 0)
                    break;
            }
        }

        public static TransactionItem[] CreateTransactions(this TransferItem transferItem)
        {
            return CreateTransactions(transferItem, null);
        }

        public static TransactionItem[] CreateTransactions(this TransferItem transferItem, string remainderAddress, params AddressItem[] fromAddressItems)
        {
            if (fromAddressItems == null)
                fromAddressItems = new AddressItem[] { };

            var totalBalance = fromAddressItems.Sum(f => f.Balance);
            var needBalance = transferItem.Value; //transferItems.Sum(t => t.Value);
            if (needBalance > totalBalance)
                throw new NotEnoughBalanceException(needBalance, totalBalance);

            //InputValidator.CheckTransferArray(transferItems);
            if (!InputValidator.IsValidTransfer(transferItem))
                throw new IotaException("transfer not valid");

            var depositTransaction = CreateDepositTransaction(transferItem).ToArray();

            TransactionItem[] withdrawalTransactions = null;
            if (needBalance > 0)
            {
                withdrawalTransactions = CreateWithdrawalTransactions(needBalance, remainderAddress, fromAddressItems)
                    .ToArray();
            }

            var transactions = new List<TransactionItem>();
            transactions.AddRange(depositTransaction);

            if (withdrawalTransactions != null)
                transactions.AddRange(withdrawalTransactions);

            var bundleHash = transactions.FinalizeAndNormalizeBundleHash(new Curl());

            if (withdrawalTransactions != null)
            {
                withdrawalTransactions.SignSignatures(fromAddressItems);
            }

            var transactionsBalance = transactions.Sum(t => Int64.Parse(t.Value));
            if (transactionsBalance != 0)
                throw new IotaException($"Total transactions balance should be zero. Current is '{transactionsBalance}'. There is some bug in code.");

            return transactions.ToArray();
        }
    }
}
