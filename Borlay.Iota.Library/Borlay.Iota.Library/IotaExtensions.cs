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
        /// <summary>
        /// Sends transfer with money.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="transferItem">Transfer item to send</param>
        /// <param name="seed">Seed from which you want to send money.</param>
        /// <param name="startFromIndex">Index to start search for address</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static async Task<TransactionItem[]> SendTransfer(this IotaApi api, TransferItem transferItem, string seed, int startFromIndex, CancellationToken cancellationToken)
        {
            var transactionItemsToSend = await api.CreateTransactions(transferItem, seed, startFromIndex, cancellationToken);
            var transactionItems = await api.AttachTransactions(transactionItemsToSend, cancellationToken);
            return transactionItems;
        }

        /// <summary>
        /// Sends transfer without money
        /// </summary>
        /// <param name="api"></param>
        /// <param name="transferItem">Transfer item</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static Task<TransactionItem[]> SendTransfer(this IotaApi api, TransferItem transferItem, CancellationToken cancellationToken)
        {
            var transactionItems = transferItem.CreateTransactions();
            return api.AttachTransactions(transactionItems, cancellationToken);
        }

        public static string[] GetTrytes(this IEnumerable<TransactionItem> transactions)
        {
            var transactionTrytes = transactions
                .OrderByDescending(o => o.CurrentIndex)
                .Select(t => t.ToTransactionTrytes())
                .ToArray();
            return transactionTrytes;
        }

        //public static async Task<TransactionItem> GetTransactions(this AddressItem)

        private static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                yield return string.Empty;
                yield break;
            }

            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static IEnumerable<TransactionItem> CreateDepositTransaction(TransferItem transferItem)
        {
            var timestamp = IotaUtils.CreateTimeStampNow().ToString();
            var tag = transferItem.Tag.ValidateTrytes(nameof(transferItem.Tag)).Pad(27);

            var messages = ChunksUpto(transferItem.Message, 2187).ToArray();
            for (int i = 0; i < messages.Length; i++)
            {
                var message = messages[i].ValidateTrytes(nameof(transferItem.Message)).Pad(2187);

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

        private static IEnumerable<TransactionItem> CreateWithdrawalTransactions(string tag, long withdrawAmount, string reminderAddress, params AddressItem[] addressItems)
        {
            if (string.IsNullOrWhiteSpace(reminderAddress))
                throw new ArgumentNullException(nameof(reminderAddress));

            var curl = new Curl();
            tag = tag.ValidateTrytes(nameof(tag)).Pad(27);

            foreach (var addressItem in addressItems)
            {
                if (addressItem.Balance <= 0)
                    continue;

                var timestamp = IotaUtils.CreateTimeStampNow().ToString();

                var amount = addressItem.Balance;
                withdrawAmount -= amount;

                var transactionItem = new TransactionItem()
                {
                    Address = addressItem.Address,
                    Value = (-amount).ToString(), // withdraw all amount
                    Timestamp = timestamp,
                    Tag = tag
                };
                yield return transactionItem;

                transactionItem = new TransactionItem()
                {
                    Address = addressItem.Address,
                    Value = "0",
                    Timestamp = timestamp,
                    Tag = tag
                };
                yield return transactionItem;



                if (withdrawAmount < 0) // deposit remind amount to reminder address
                {
                    var message = "".Pad(2187);

                    transactionItem = new TransactionItem()
                    {
                        Address = reminderAddress,
                        Value = Math.Abs(withdrawAmount).ToString(),
                        Timestamp = timestamp,
                        SignatureFragment = message,
                        Tag = tag
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

            InputValidator.ValidateTransfer(transferItem);

            var depositTransaction = CreateDepositTransaction(transferItem).ToArray();

            TransactionItem[] withdrawalTransactions = null;
            if (needBalance > 0)
            {
                withdrawalTransactions = CreateWithdrawalTransactions(transferItem.Tag, needBalance, remainderAddress, fromAddressItems)
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

        public static Task<string[]> DoPow(this string[] trytes, string trunkTransaction, string branchTransaction, int minWeightMagnitude, CancellationToken cancellationToken)
        {
            return trytes.DoPow(trunkTransaction, branchTransaction, minWeightMagnitude, 0, cancellationToken);
        }

        public static async Task<string[]> DoPow(this string[] trytes, string trunkTransaction, string branchTransaction, int minWeightMagnitude, int numberOfThreads, CancellationToken cancellationToken)
        {
            var trunk = trunkTransaction;
            var branch = branchTransaction;

            List<string> resultTrytes = new List<string>();
            for (int i = 0; i < trytes.Length; i++)
            {
                if (i == 0)
                    branch = branchTransaction;
                else
                    branch = trunkTransaction;

                var tryte = trytes[i];
                tryte = tryte.SetApproveTransactions(trunk, branch);

                var diver = new PowDiver();
                var tryteWithNonce = await diver.DoPow(tryte, minWeightMagnitude, numberOfThreads, cancellationToken);
                var transaction = new TransactionItem(tryteWithNonce);
                trunk = transaction.Hash;

                resultTrytes.Add(tryteWithNonce);
            }
            return resultTrytes.ToArray();
        }

        public static string Pad(this string value, int size)
        {
            return value.Pad('9', size);
        }

        public static string PadLast(this string value, int size)
        {
            value = value ?? "";
            if (string.IsNullOrWhiteSpace(value))
                value = "9";

            var lastSimbol = value[value.Length - 1];
            return value.Pad(lastSimbol, size);
        }

        public static string Pad(this string value, char simbol, int size)
        {
            var pad = value ?? "";
            while (pad.Length < size) pad += simbol;
            return pad;
        }

        public static string SetApproveTransactions(this string trytes, string trunkTransaction, string branchTransaction)
        {
            var trytesConstruct = trytes.Substring(0, 2430);
            trytesConstruct += trunkTransaction;
            trytesConstruct += branchTransaction;
            trytesConstruct += EmptyHash();

            return trytesConstruct;
        }

        public static string SetApproveTrunk(this string trytes, string trunkTransaction)
        {
            var branchTransaction = trytes.GetBranchTransaction();
            var trytesConstruct = trytes.Substring(0, 2430);
            trytesConstruct += trunkTransaction;
            trytesConstruct += branchTransaction;
            trytesConstruct += EmptyHash();

            return trytesConstruct;
        }

        public static string SetApproveBranch(this string trytes, string branchTransaction)
        {
            var trytesConstruct = trytes.Substring(0, 2430 + 81);
            trytesConstruct += branchTransaction;
            trytesConstruct += EmptyHash();

            return trytesConstruct;
        }

        public static string GetTrunkTransaction(this string trytes)
        {
            return trytes.Substring(2430, 81);
        }

        public static string GetBranchTransaction(this string trytes)
        {
            return trytes.Substring(2430 + 81, 81);
        }

        public static string EmptyHash(int length = 81)
        {
            var emptyHash = new string(Enumerable.Repeat('9', 81).ToArray());
            return emptyHash;
        }
    }
}
