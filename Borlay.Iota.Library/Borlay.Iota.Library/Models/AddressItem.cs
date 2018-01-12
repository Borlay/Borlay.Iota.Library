using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    /// <summary>
    /// This class represents an AddressInfo
    /// </summary>
    public class AddressItem : AddressModelBase
    {
        private sbyte[] privateKey;
        private long balance;
        private int keyIndex;
        //private bool hasTransactions;

        public AddressItem()
        {
            this.transactions = new ObservableCollection<TransactionHash>();
            this.transactions.CollectionChanged += Transactions_CollectionChanged;
        }

        private void Transactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TransactionCount));
        }

        public int TransactionCount
        {
            get
            {
                return this.Transactions?.Count ?? 0;
            }
        }

        /// <summary>
        /// Gets or sets the private key
        /// </summary>
        public virtual sbyte[] PrivateKey
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

        private ObservableCollection<TransactionHash> transactions;

        /// <summary>
        /// Gets the transactions
        /// </summary>
        public ObservableCollection<TransactionHash> Transactions
        {
            get
            {
                return transactions;
            }
        }
    }
}
