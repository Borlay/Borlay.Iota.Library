using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    public interface IApproveTransactions
    {
        /// <summary>
        /// Gets or sets the trunk transaction.
        /// </summary>
        /// <value>
        /// The trunk transaction.
        /// </value>
        string TrunkTransaction { get; }

        /// <summary>
        /// Gets or sets the branch transaction.
        /// </summary>
        /// <value>
        /// The branch transaction.
        /// </value>
        string BranchTransaction { get; }
    }
}
