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
        private readonly IriApi iriApi;

        /// <summary>
        /// Gets the IriApi
        /// </summary>
        public IriApi IriApi => iriApi;

        /// <summary>
        /// Gets or sets ExceptionHandler
        /// </summary>
        public ITransactionHandler ExceptionHandler { get; set; }

        /// <summary>
        /// Gets the Url
        /// </summary>
        public string Url => iriApi.WebClient.Url;

        public IotaApi(string url)
            : this(new IriApi(url))
        {
        }

        public IotaApi(IriApi iriApi)
        {
            if (iriApi == null)
                throw new ArgumentNullException(nameof(iriApi));

            this.iriApi = iriApi;
        }

        /// <summary>
        /// Generates and address and checks for hasTransaction and balance
        /// </summary>
        /// <param name="seed">Seed</param>
        /// <param name="index">Address index</param>
        /// <returns></returns>
        public async Task<AddressItem> GetAddress(string seed, int index)
        {
            InputValidator.CheckIfValidSeed(seed);
            seed = InputValidator.PadSeedIfNecessary(seed);

            var addressItem = await NewAddressAsync(seed, index, CancellationToken.None);
            await iriApi.RenewBalances(addressItem);
            return addressItem;
        }

        /// <summary>
        /// Generates an addresses and checks for transactions and balances
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
                var task = NewAddressAsync(seed, i, cancellationToken);
                tasks.Add(task);
            }
            var addressItems = await Task.WhenAll(tasks);

            cancellationToken.ThrowIfCancellationRequested();

            var renewBalances = addressItems.Where(a => a.HasTransactions).ToArray();
            if (renewBalances.Length > 0)
                await RenewBalances(renewBalances);

            return addressItems.ToArray();
        }

        private async Task<AddressItem> NewAddressAsync(string seed, int index, CancellationToken cancellationToken)
        {
            await Task.Yield();
            // need yield because new address generation is very costly

            int[] key = new Signing(new Curl()).Key(Converter.ToTrits(seed), index, 2);
            string address = IotaApiUtils.NewAddress(key, false, new Curl(), cancellationToken);

            var addressItem = new AddressItem()
            {
                Address = address,
                PrivateKey = key,
                Index = index,
                HasTransactions = false,
                Balance = 0
            };

            await RenewHasTransaction(addressItem);

            return addressItem;
        }

        /// <summary>
        /// Renew hasTransaction property for address item
        /// </summary>
        /// <param name="addressItem">Address item to renew</param>
        /// <returns></returns>
        public async Task RenewHasTransaction(AddressItem addressItem)
        {
            if (!addressItem.HasTransactions) // because once you put in tangle it's always true
            {
                var transactionHashes = await iriApi.FindTransactionsFromAddresses(addressItem.Address);
                addressItem.HasTransactions = (transactionHashes?.Length > 0);
            }
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
                tasks.Add(RenewHasTransaction(item));
            }
            await Task.WhenAll(tasks);

            var renewBalances = addressItems.Where(a => a.HasTransactions).ToArray();
            if (renewBalances.Length > 0)
                await RenewBalances(renewBalances);
        }

        public async Task<AddressItem> FindReminderAddress(string seed, int startFromIndex, CancellationToken cancellationToken)
        {
            for (int i = startFromIndex; i < 500; i += 10)
            {
                var addressItems = await GetAddresses(seed, i, 10, cancellationToken);
                foreach (var addressItem in addressItems)
                {
                    if (addressItem.Balance > 0 || !addressItem.HasTransactions)
                        return addressItem;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            throw new IotaException("Remainder address not found. Please choice it manually.");
        }

        public async Task<AddressItem[]> FindAddressesWithBalance(string seed, int startFromIndex, long? withdrawalAmount, CancellationToken cancellationToken)
        {
            long totalBalance = 0;
            List<AddressItem> addressList = new List<AddressItem>();
            for (int i = startFromIndex; i < 500; i += 10)
            {
                var addressItems = await GetAddresses(seed, i, 10, cancellationToken);
                if (addressItems.All(a => !a.HasTransactions))
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
        public async Task<TransactionItem[]> SendTransactions(TransferItem transferItem, IEnumerable<AddressItem> addressItems, string remainderAddress, CancellationToken cancellationToken)
        {
            await RenewBalances(addressItems.ToArray());
            return await SendTransferWithoutRenewBalance(transferItem, addressItems, remainderAddress, cancellationToken);
        }

        /// <summary>
        /// Sends transfer without renew balance
        /// </summary>
        /// <param name="transferItem">The transfer items to send</param>
        /// <param name="addressItems">The address items from which amount is send</param>
        /// <param name="remainderAddress">The remainder where remaind amount is send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public async Task<TransactionItem[]> SendTransferWithoutRenewBalance(TransferItem transferItem, IEnumerable<AddressItem> addressItems, string remainderAddress, CancellationToken cancellationToken)
        {
            var transactionItems = transferItem.CreateTransactions(remainderAddress, addressItems.ToArray());
            var resultTransactionItems = await SendTransactions(transactionItems, cancellationToken);
            return resultTransactionItems;
        }

        /// <summary>
        /// Attach transactions to tangle, broadcast and store.
        /// </summary>
        /// <param name="transactions">Transactions to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<TransactionItem[]> SendTransactions(IEnumerable<TransactionItem> transactions, CancellationToken cancellationToken)
        {
            var transactionTrytes = transactions.GetTrytes();
            var trytes = await SendTrytes(transactionTrytes, cancellationToken);
            var transactionResult = trytes.Select(t => new TransactionItem(t)).ToArray();
            return transactionResult;
        }

        /// <summary>
        /// Attach, broadcast and store trytes to tangle
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<string[]> SendTrytes(string[] trytes, CancellationToken cancellationToken)
        {
            string[] trytesResult;
            try
            {
                trytesResult = await iriApi.AttachToTangle(trytes, cancellationToken);
                if(ExceptionHandler != null)
                    await ExceptionHandler.SuccessToAttachToTangleAsync(this, trytes);
            }
            catch (Exception e)
            {
                if (ExceptionHandler != null)
                {
                    var handlerResult = await ExceptionHandler.FailedToAttachToTangleAsync(this, trytes, e);
                    if (handlerResult == HandlerResult.Handled)
                        return new string[] { };
                }
                throw;
            }
            await BroadcastAndStore(trytesResult);
            return trytesResult;
        }

        /// <summary>
        /// Broadcasts and stores the trytes
        /// </summary>
        /// <param name="trytes">Trytes to send</param>
        /// <returns></returns>
        public virtual async Task BroadcastAndStore(string[] trytes)
        {
            try
            {
                await iriApi.BroadcastTransactions(trytes);
                await iriApi.StoreTransactions(trytes);
                if (ExceptionHandler != null)
                    await ExceptionHandler.SuccessToBroadcastAndStoreAsync(this, trytes);
            }
            catch (Exception e)
            {
                if (ExceptionHandler != null)
                {
                    var handlerResult = await ExceptionHandler.FailedToBroadcastAndStoreAsync(this, trytes, e);
                    if (handlerResult == HandlerResult.Handled)
                        return;
                }
                throw;
            }
        }
    }
}
