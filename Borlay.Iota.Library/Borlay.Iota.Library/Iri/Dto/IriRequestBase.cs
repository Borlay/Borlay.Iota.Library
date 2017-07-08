using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class serves as base class for the different core API calls/requests
    /// </summary>
    public class IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IriRequestBase"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public IriRequestBase(string command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public string Command { get; set; }
    }
}
