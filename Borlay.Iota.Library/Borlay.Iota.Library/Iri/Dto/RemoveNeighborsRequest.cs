using System.Collections.Generic;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core api request 'RemoveNeighbors'
    /// </summary>
    /// <seealso cref="Borlay.Iota.Library.Iri.Dto.IriRequestBase" />
    public class RemoveNeighborsRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveNeighborsRequest"/> class.
        /// </summary>
        /// <param name="uris">The uris.</param>
        public RemoveNeighborsRequest(string[] uris) : base(CommandConstants.RemoveNeighbors)
        {
            Uris = uris;
        }

        /// <summary>
        /// Gets or sets the uris of the neighbours to remove
        /// </summary>
        /// <value>
        /// The uris of the neighbours to remove.
        /// </value>
        public string[] Uris { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Uris)}: {string.Join(",", Uris)}";
        }
    }
}