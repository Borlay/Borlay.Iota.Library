using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    /// <summary>
    /// This class represents a neigbhor
    /// </summary>
    public class Neighbor : AddressModelBase
    {
        private long numberOfAllTransactions;
        private long numberOfInvalidTransactions;
        private long numberOfNewTransactions;

        /// <summary>
        /// Gets or sets the number of all transactions.
        /// </summary>
        /// <value>
        /// The number of all transactions.
        /// </value>
        public long NumberOfAllTransactions
        {
            get
            {
                return this.numberOfAllTransactions;
            }
            set
            {
                if (value != this.numberOfAllTransactions)
                {
                    this.numberOfAllTransactions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of invalid transactions.
        /// </summary>
        /// <value>
        /// The number of invalid transactions.
        /// </value>
        public long NumberOfInvalidTransactions
        {
            get
            {
                return this.numberOfInvalidTransactions;
            }
            set
            {
                if (value != this.numberOfInvalidTransactions)
                {
                    this.numberOfInvalidTransactions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of new transactions.
        /// </summary>
        /// <value>
        /// The number of new transactions.
        /// </value>
        public long NumberOfNewTransactions
        {
            get
            {
                return this.numberOfNewTransactions;
            }
            set
            {
                if (value != this.numberOfNewTransactions)
                {
                    this.numberOfNewTransactions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(NumberOfAllTransactions)}: {NumberOfAllTransactions}, {nameof(NumberOfInvalidTransactions)}: {NumberOfInvalidTransactions}, {nameof(NumberOfNewTransactions)}: {NumberOfNewTransactions}";
        }
    }
}
