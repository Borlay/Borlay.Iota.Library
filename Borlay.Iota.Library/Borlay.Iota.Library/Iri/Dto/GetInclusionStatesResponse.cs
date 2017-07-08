using System.Collections.Generic;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the response of <see cref="GetInclusionStatesRequest"/>
    /// </summary>
    /// <seealso cref="Borlay.Iota.Library.Iri.Dto.IriResponseBase" />
    public class GetInclusionStatesResponse : IriResponseBase
    {
        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        /// <value>
        /// The states.
        /// </value>
        public bool[] States { get; set; }
    }
}