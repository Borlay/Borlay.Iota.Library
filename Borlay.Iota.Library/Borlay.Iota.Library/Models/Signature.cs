using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    /// <summary>
    /// Thic class represents a signature
    /// </summary>
    public class Signature : AddressModelBase
    {
        private string[] signatureFragments;

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        public Signature()
        {
            this.SignatureFragments = new string[] { };
        }

        /// <summary>
        /// Gets or sets the signature fragments.
        /// </summary>
        /// <value>
        /// The signature fragments.
        /// </value>
        public string[] SignatureFragments { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(SignatureFragments)}: {string.Join(",", SignatureFragments)}";
        }
    }
}
