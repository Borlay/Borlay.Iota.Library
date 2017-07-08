using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    public class TransactionBundle
    {
        public TransactionItem[] Transactions { get; set; }


        public TransactionBundle(params TransactionItem[] transactionItems)
        {
            if (transactionItems == null)
                throw new ArgumentNullException(nameof(transactionItems));

            this.Transactions = transactionItems;
        }

        /// <summary>
        /// Gets the length of the bundle
        /// </summary>
        public int Length => Transactions.Length;
    }
}
