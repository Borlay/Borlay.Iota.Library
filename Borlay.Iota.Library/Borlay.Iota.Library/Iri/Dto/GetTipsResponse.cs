using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the response of <see cref="GetTipsRequest"/>
    /// </summary>
    public class GetTipsResponse : IriResponseBase
    {
        /// <summary>
        /// Gets or sets the hashes.
        /// </summary>
        /// <value>
        /// The hashes.
        /// </value>
        public string[] Hashes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Hashes)}: {string.Join(",", Hashes)}";
        }
    }
}
