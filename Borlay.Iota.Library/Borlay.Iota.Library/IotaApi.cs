﻿using Borlay.Iota.Library.Crypto;
using Borlay.Iota.Library.Exceptions;
using Borlay.Iota.Library.Iri;
using Borlay.Iota.Library.Models;
using Borlay.Iota.Library.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library
{
    public class IotaApi
    {
        /// <summary>
        /// Gets or sets MaxAddressIndex. Default is 500.
        /// </summary>
        public int MaxAddressIndex { get; set; }

        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets or sets the NonceSeeker
        /// </summary>
        public INonceSeeker NonceSeeker { get; set; }

        /// <summary>
        /// Gets or sets address security. Default 2
        /// </summary>
        public int AddressSecurity { get; set; }

        /// <summary>
        /// Gets or sets RebroadcastMaximumPowTime in milliseconds
        /// </summary>
        public int RebroadcastMaximumPowTime { get; set; }

        private readonly IriApi iriApi;

        /// <summary>
        /// Gets the IriApi
        /// </summary>
        public IriApi IriApi => iriApi;

        /// <summary>
        /// Gets the Url
        /// </summary>
        public string Url => iriApi.WebClient.Url;

        public IotaApi(string url, int minWeightMagnitude = 14)
            : this(new IriApi(url) { MinWeightMagnitude = minWeightMagnitude })
        {
        }

        public IotaApi(string url, INonceSeeker nonceSeeker, int minWeightMagnitude = 14)
            : this(new IriApi(url) { MinWeightMagnitude = minWeightMagnitude }, nonceSeeker)
        {
        }

        public IotaApi(IriApi iriApi)
            : this(iriApi, new LocalNonceSeeker(iriApi.MinWeightMagnitude))
        {
        }

        public IotaApi(IriApi iriApi, INonceSeeker nonceSeeker)
        {
            if (iriApi == null)
                throw new ArgumentNullException(nameof(iriApi));

            if (nonceSeeker == null)
                throw new ArgumentNullException(nameof(nonceSeeker));

            this.iriApi = iriApi;
            this.MaxAddressIndex = 500;
            this.Depth = 9;
            this.AddressSecurity = 2;
            this.RebroadcastMaximumPowTime = 20000;
            this.NonceSeeker = nonceSeeker;
        }

        /// <summary>
        /// Generates and address and gets transactions and balance
        /// </summary>
        /// <param name="seed">Seed</param>
        /// <param name="index">Address index</param>
        /// <returns></returns>
        public async Task<AddressItem> GetAddress(string seed, int index)
        {
            InputValidator.CheckIfValidSeed(seed);
            seed = InputValidator.PadSeedIfNecessary(seed);

            var addressItem = await GenerateAddressAsync(seed, index, CancellationToken.None);
            await iriApi.RenewBalances(addressItem);
            return addressItem;
        }

        /// <summary>
        /// Generates an addresses and gets transactions and balances
        /// </summary>
        /// <param name="seed">Seed</param>
        /// <param name="index">Address index to start</param>
        /// <param name="count">Address count to generate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<AddressItem[]> GetAddresses(string seed, int index, int count, CancellationToken cancellationToken)
        {
            InputValidator.CheckIfValidSeed(seed);
            seed = InputValidator.PadSeedIfNecessary(seed);

            var tasks = new List<Task<AddressItem>>();
            for (var i = index; i < count + index; i++)
            {
                var task = GenerateAddressAsync(seed, i, cancellationToken);
                tasks.Add(task);
            }
            var addressItems = await Task.WhenAll(tasks);

            cancellationToken.ThrowIfCancellationRequested();

            var renewBalances = addressItems.Where(a => a.TransactionCount > 0).ToArray();
            if (renewBalances.Length > 0)
                await RenewBalances(renewBalances);

            return addressItems.ToArray();
        }



        /// <summary>
        /// Generates an address and gets transactions.
        /// </summary>
        /// <param name="seed">The seed from which an address should be generated</param>
        /// <param name="index">The index of the address</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        private async Task<AddressItem> GenerateAddressAsync(string seed, int index, CancellationToken cancellationToken)
        {
            await TaskIota.Yield().ConfigureAwait(false);
            // need yield because new address generation is very costly

            var addressItem = IotaUtils.GenerateAddress(seed, index, AddressSecurity, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            await RenewTransactions(addressItem);
            return addressItem;
        }

        /// <summary>
        /// Renew hasTransaction property for address item
        /// </summary>
        /// <param name="addressItem">Address item to renew</param>
        /// <returns></returns>
        public async Task RenewTransactions(AddressItem addressItem)
        {
            var transactionHashes = await iriApi.FindTransactionsFromAddresses(addressItem.Address);
            foreach (var hash in transactionHashes)
            {
                if (!addressItem.Transactions.Any(t => t.Hash == hash))
                {
                    addressItem.Transactions.Add(new TransactionHash() { Hash = hash });
                }
            }
        }

        /// <summary>
        /// Gets transactions details and inclusion states by transactions hashes
        /// </summary>
        /// <param name="transactionHashes">The transactions hashes</param>
        /// <returns></returns>
        public async Task<TransactionItem[]> GetTransactionItems(params string[] transactionHashes)
        {
            var transactionTrytes = await iriApi.GetTrytes(transactionHashes);
            var nodeInfo = await iriApi.GetNodeInfo();
            var states = await iriApi.GetInclusionStates(transactionHashes, nodeInfo.LatestSolidSubtangleMilestone);

            var transactionItems = transactionTrytes.Select(t => new TransactionItem(t)).ToArray();

            for (int i = 0; i < transactionItems.Length; i++)
            {
                transactionItems[i].Persistence = states[i];
            }

            return transactionItems;
        }
        /// <summary>
        /// Gets transactions details and inclusion states by bundle hash
        /// </summary>
        /// <param name="bundleHash"></param>
        /// <returns></returns>
        public async Task<TransactionItem[]> GetBundleTransactionItems(string bundleHash)
        {
            var transactionHashes = await iriApi.FindTransactions(null, null, null, new string[] { bundleHash });
            var transactionItems = await GetTransactionItems(transactionHashes);
            return transactionItems;
        }

        /// <summary>
        /// Renew address items balances
        /// </summary>
        /// <param name="addressItems">Address items to renew</param>
        /// <returns></returns>
        public Task RenewBalances(params AddressItem[] addressItems)
        {
            return IriApi.RenewBalances(addressItems);
        }

        /// <summary>
        /// Renew address hasTransaction and balance
        /// </summary>
        /// <param name="addressItems">Address items to renew</param>
        /// <returns></returns>
        public async Task RenewAddresses(params AddressItem[] addressItems)
        {
            var tasks = new List<Task>();
            foreach (var item in addressItems)
            {
                tasks.Add(RenewTransactions(item));
            }
            await Task.WhenAll(tasks);

            var renewBalances = addressItems.Where(a => a.TransactionCount > 0).ToArray();
            if (renewBalances.Length > 0)
                await RenewBalances(renewBalances);
        }

        /// <summary>
        /// Find reminder address.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="startFromIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AddressItem> FindReminderAddress(string seed, int startFromIndex, CancellationToken cancellationToken)
        {
            for (int i = startFromIndex; i < MaxAddressIndex; i++)
            {
                var addressItem = await GetAddress(seed, i);
                if (addressItem.Balance > 0 || addressItem.TransactionCount == 0)
                    return addressItem;

                cancellationToken.ThrowIfCancellationRequested();
            }

            throw new IotaException("Remainder address not found. Please choice it manually.");
        }

        public async Task<AddressItem[]> FindAddressesWithBalance(string seed, int startFromIndex, long? withdrawalAmount, CancellationToken cancellationToken)
        {
            long totalBalance = 0;
            List<AddressItem> addressList = new List<AddressItem>();
            for (int i = startFromIndex; i < MaxAddressIndex; i++)
            {
                var addressItems = await GetAddresses(seed, i, 1, cancellationToken);
                if (addressItems.All(a => a.TransactionCount == 0))
                    break;

                foreach (var addressItem in addressItems)
                {
                    if (addressItem.Balance > 0)
                    {
                        addressList.Add(addressItem);
                        totalBalance += addressItem.Balance;

                        if (withdrawalAmount.HasValue && totalBalance >= withdrawalAmount.Value)
                            return addressList.ToArray();
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            if (withdrawalAmount.HasValue)
                throw new NotEnoughBalanceException(withdrawalAmount.Value, totalBalance);

            return addressList.ToArray();
        }


        /// <summary>
        /// Finds addresses with balance and finds reminder and creates transaction items
        /// </summary>
        /// <param name="transferItem"></param>
        /// <param name="seed"></param>
        /// <param name="startFromIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TransactionItem[]> CreateTransactions(TransferItem transferItem, string seed, int startFromIndex, CancellationToken cancellationToken)
        {
            var withdrawalAmount = transferItem.Value;
            var addressItems = await FindAddressesWithBalance(seed, startFromIndex, withdrawalAmount, cancellationToken);
            return await CreateTransactions(transferItem, seed, addressItems, cancellationToken);
        }

        /// <summary>
        /// Finds reminder and creates transaction items
        /// </summary>
        /// <param name="transferItem"></param>
        /// <param name="seed"></param>
        /// <param name="addressItems"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TransactionItem[]> CreateTransactions(TransferItem transferItem, string seed, IEnumerable<AddressItem> addressItems, CancellationToken cancellationToken)
        {
            addressItems = addressItems.Where(b => b.Balance > 0).ToArray();
            var reminderAddressItem = await FindReminderAddress(seed, addressItems.Max(a => a.Index) + 1, cancellationToken);
            var transactionItems = transferItem.CreateTransactions(reminderAddressItem.Address, addressItems.ToArray());
            return transactionItems;
        }

        /// <summary>
        /// Renew balance and sends transfer items
        /// </summary>
        /// <param name="transferItem">The transfer items to send</param>
        /// <param name="addressItems">The address items from which amount is send</param>
        /// <param name="remainderAddress">The remainder where remaind amount is send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public async Task<TransactionItem[]> AttachTransfer(TransferItem transferItem, IEnumerable<AddressItem> addressItems, string remainderAddress, CancellationToken cancellationToken)
        {
            await RenewBalances(addressItems.ToArray());
            return await AttachTransferWithoutRenewBalance(transferItem, addressItems, remainderAddress, cancellationToken);
        }

        /// <summary>
        /// Sends transfer without renew balance
        /// </summary>
        /// <param name="transferItem">The transfer items to send</param>
        /// <param name="addressItems">The address items from which amount is send</param>
        /// <param name="remainderAddress">The remainder where remaind amount is send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public async Task<TransactionItem[]> AttachTransferWithoutRenewBalance(TransferItem transferItem, IEnumerable<AddressItem> addressItems, string remainderAddress, CancellationToken cancellationToken)
        {
            var transactionItems = transferItem.CreateTransactions(remainderAddress, addressItems.ToArray());
            var resultTransactionItems = await AttachTransactions(transactionItems, cancellationToken);
            return resultTransactionItems;
        }

        /// <summary>
        /// Attach transactions to tangle, broadcast and store.
        /// </summary>
        /// <param name="transactions">Transactions to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<TransactionItem[]> AttachTransactions(IEnumerable<TransactionItem> transactions, CancellationToken cancellationToken)
        {
            var transactionTrytes = transactions.GetTrytes();
            var trytes = await AttachTrytes(transactionTrytes, cancellationToken);
            var transactionResult = trytes.Select(t => new TransactionItem(t)).ToArray();
            return transactionResult;
        }

        /// <summary>
        /// Fast transaction reattach. It approves only branch transaction.
        /// </summary>
        /// <param name="transactionItems">Transaction items to reattach</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task<string> FastReattach(IEnumerable<TransactionItem> transactionItems, CancellationToken cancellationToken)
        {
            var transactionItem = transactionItems.FirstOrDefault(t => t.CurrentIndex == "0");

            if (transactionItem == null)
                throw new Exception("Transaction with index '0' not found");

            var transactionTrytes = transactionItem.ToTransactionTrytes();
            var trytesResult = await FastReattach(transactionTrytes, cancellationToken);
            return trytesResult;
        }

        /// <summary>
        /// Fast transaction reattach. It approves only branch transaction.
        /// </summary>
        /// <param name="trytes">Trytes to rebroadcast</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task<string> FastReattach(string trytes, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(trytes))
                throw new ArgumentNullException(nameof(trytes));

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            if (NonceSeeker == null)
                throw new NullReferenceException(nameof(NonceSeeker));

            while (true)
            {
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    cancellationToken.Register(() => cts.Cancel());

                    var toApprove = await IriApi.GetTransactionsToApprove(Depth);
                    cts.CancelAfter(RebroadcastMaximumPowTime);

                    var trunk = trytes.GetTrunkTransaction();
                    var branch = toApprove.TrunkTransaction;

                    var trytesToSend = (await NonceSeeker
                        .SearchNonce(new string[] { trytes }, trunk, branch, cts.Token))
                        .Single();

                    cts.Token.ThrowIfCancellationRequested();

                    await IriApi.BroadcastTransactions(trytesToSend);
                    await IriApi.StoreTransactions(trytesToSend);

                    var transaction = new TransactionItem(trytesToSend);
                    return transaction.Hash;
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw;

                    continue;
                }
            }
        }

        /// <summary>
        /// Approve transactions, do pow, broadcast and store trytes to tangle
        /// </summary>
        /// <param name="transactionTrytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<string[]> AttachTrytes(string[] transactionTrytes, CancellationToken cancellationToken)
        {
            if (transactionTrytes == null)
                throw new ArgumentNullException(nameof(transactionTrytes));

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            if (NonceSeeker == null)
                throw new NullReferenceException(nameof(NonceSeeker));

            var toApprove = await IriApi.GetTransactionsToApprove(Depth);

            var trunk = toApprove.TrunkTransaction;
            var branch = toApprove.BranchTransaction;

            var trytesToSend = await NonceSeeker.SearchNonce(transactionTrytes, trunk, branch, cancellationToken);

            await BroadcastAndStore(trytesToSend);
            return trytesToSend;
        }

        /// <summary>
        /// Broadcasts and stores the trytes
        /// </summary>
        /// <param name="trytes">Trytes to send</param>
        /// <returns></returns>
        public virtual async Task BroadcastAndStore(string[] trytes)
        {
            await iriApi.BroadcastTransactions(trytes);
            await iriApi.StoreTransactions(trytes);
        }
    }
}
