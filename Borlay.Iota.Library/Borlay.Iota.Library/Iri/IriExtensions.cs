using Borlay.Iota.Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Iri
{
    public static class IriExtensions
    {
        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        public static Task<TResponse> RequestAsync<TResponse>(this IGenericWebClient client, object request)
            where TResponse : new()
        {
            return client.RequestAsync<TResponse>(request, CancellationToken.None);
        }

        public static async Task<string[]> FindTransactionsFromAddresses(this IriApi api,
            params string[] addresses)
        {
            var transactionTrytes = await api.FindTransactions(addresses, null, null, null);
            return transactionTrytes;
        }

        public static async Task<TransactionItem[]> FindTransactionItemsFromAddresses(this IriApi api,
            params string[] addresses)
        {
            var transactionHashes = await api.FindTransactionsFromAddresses(addresses);
            return await api.FindTransactionItemsFromHashes(transactionHashes);
        }

        public static async Task<TransactionItem[]> FindTransactionItemsFromHashes(this IriApi api,
            params string[] hashes)
        {
            var transactionTrytes = await api.GetTrytes(hashes);
            return transactionTrytes.Select(t => new TransactionItem(t)).ToArray();
        }

        /// <summary>
        /// Renew address items balances
        /// </summary>
        /// <param name="api">Iri api</param>
        /// <param name="addressItems">Address items to renew</param>
        /// <returns></returns>
        public static async Task RenewBalances(this IriApi api, params AddressItem[] addressItems)
        {
            if (addressItems.Length == 0) return;
            var addresses = addressItems.Select(a => a.Address).ToArray();
            var balances = await api.GetBalances(addresses);
            for (int i = 0; i < addressItems.Length; i++)
            {
                addressItems[i].Balance = balances[i];
            }
        }

        public static Task<long[]> GetBalances(this IriApi api, params string[] addresses)
        {
            return api.GetBalances(addresses, 100);
        }
    }
}
