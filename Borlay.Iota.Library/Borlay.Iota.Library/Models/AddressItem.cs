using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    /// <summary>
    /// This class represents an AddressInfo
    /// </summary>
    public class AddressItem : AddressModelBase
    {
        private int[] privateKey;
        private long balance;
        private int keyIndex;
        private bool hasTransactions;

        public bool HasTransactions
        {
            get
            {
                return this.hasTransactions;
            }
            set
            {
                if (value != this.hasTransactions)
                {
                    this.hasTransactions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the private key
        /// </summary>
        public int[] PrivateKey
        {
            get
            {
                return this.privateKey;
            }
            set
            {
                if (value != this.privateKey)
                {
                    this.privateKey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public long Balance
        {
            get
            {
                return this.balance;
            }
            set
            {
                if (value != this.balance)
                {
                    this.balance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the key.
        /// </summary>
        /// <value>
        /// The index of the key.
        /// </value>
        public int Index
        {
            get
            {
                return this.keyIndex;
            }
            set
            {
                if (value != this.keyIndex)
                {
                    this.keyIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
