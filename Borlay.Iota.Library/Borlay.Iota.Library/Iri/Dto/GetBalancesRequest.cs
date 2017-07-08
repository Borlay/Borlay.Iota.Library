using System.Collections.Generic;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core api request 'GetBalances'
    /// </summary>
    public class GetBalancesRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetBalancesRequest"/> class.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="threshold">The threshold.</param>
        public GetBalancesRequest(string[] addresses, long threshold = 100)
            : base(CommandConstants.GetBalances)
        {
            Addresses = addresses;
            Threshold = threshold;
        }

        /// <summary>
        /// Gets the threshold.
        /// </summary>
        /// <value>
        /// The threshold.
        /// </value>
        public long Threshold { get; }

        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        public string[] Addresses { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Threshold)}: {Threshold}, {nameof(Addresses)}: {string.Join(",",Addresses)}";
        }
    }
}