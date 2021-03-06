﻿using System.Collections.Generic;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// Broadcast a list of transactions to all neighbors. The input trytes for this call are provided by attachToTangle
    /// </summary>
    public class BroadcastTransactionsRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastTransactionsRequest"/> class.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        public BroadcastTransactionsRequest(string[] trytes)
            : base(CommandConstants.BroadcastTransactions)
        {
            Trytes = trytes;
        }

        /// <summary>
        /// Gets or sets the trytes representing the transactions
        /// </summary>
        /// <value>
        /// The trytes.
        /// </value>
        public string[] Trytes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Trytes)}: {string.Join(",", Trytes)}";
        }
    }
}