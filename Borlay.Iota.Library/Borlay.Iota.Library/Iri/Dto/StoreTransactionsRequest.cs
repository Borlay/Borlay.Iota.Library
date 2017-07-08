using System.Collections.Generic;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core API request 'StoreTransactions'.
    /// It stores transactions into the local storage. The trytes to be used for this call are returned by attachToTangle.
    /// </summary>
    public class StoreTransactionsRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreTransactionsRequest"/> class.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        public StoreTransactionsRequest(string[] trytes) : base(CommandConstants.StoreTransactions)
        {
            this.Trytes = trytes;
        }

        /// <summary>
        /// Gets or sets the trytes.
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
            return $"{nameof(Trytes)}: {Trytes}";
        }
    }
}