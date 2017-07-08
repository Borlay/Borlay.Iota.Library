namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core API call 'GetTransactionsToApprove'
    /// </summary>
    public class GetTransactionsToApproveRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTransactionsToApproveRequest"/> class.
        /// </summary>
        /// <param name="depth">The depth.</param>
        public GetTransactionsToApproveRequest(int depth)
            : base(CommandConstants.GetTransactionsToApprove)
        {
            Depth = depth;
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth}";
        }
    }
}