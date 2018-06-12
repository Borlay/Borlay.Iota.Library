using Borlay.Iota.Library.Crypto;
using Borlay.Iota.Library.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    /// <summary>
    /// This class represents an iota transaction
    /// </summary>
    public class TransactionItem : TransactionHash, IApproveTransactions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionItem"/> class.
        /// </summary>
        public TransactionItem()
        {
            TrunkTransaction = Constants.EmptyHash;
            BranchTransaction = Constants.EmptyHash;
            Nonce = Constants.EmptyHash;
            Tag = Constants.EmptyTag;
            SignatureFragment = Constants.EmptyHash;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionItem"/> class.
        /// </summary>
        /// <param name="trytes">The trytes representing the transaction</param>
        /// <param name="curl">The curl implementation.</param>
        /// <exception cref="System.ArgumentException">
        /// trytes must non-null
        /// or
        /// position " + i + "must not be '9'
        /// </exception>
        public TransactionItem(string trytes, ICurl curl)
        {
            UpdateFromTrytes(trytes, curl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionItem"/> class.
        /// </summary>
        /// <param name="trytes">The trytes representing the transaction</param>
        public TransactionItem(string trytes) : this(trytes, new Crypto.Curl())
        {
        }


        public void UpdateFromTrytes(string trytes)
        {
            UpdateFromTrytes(trytes, new Crypto.Curl());
        }

        public void UpdateFromTrytes(string trytes, ICurl curl)
        {
            if (string.IsNullOrEmpty(trytes))
                throw new ArgumentNullException(nameof(trytes));

            if (curl == null)
                throw new ArgumentNullException(nameof(curl));

            // validity check
            for (int i = 2279; i < 2295; i++)
            {
                if (trytes[i] != '9')
                    throw new ArgumentException("position " + i + "must not be '9'");

            }

            var transactionTrits = Crypto.Converter.GetTrits(trytes);
            sbyte[] hash = new sbyte[243];

            // generate the correct transaction hash
            curl.Reset()
                .Absorb(transactionTrits, 0, transactionTrits.Length)
                .Squeeze(hash, 0, hash.Length);

            Hash = Crypto.Converter.GetTrytes(hash);
            SignatureFragment = trytes.Substring(0, 2187);
            Address = trytes.Substring(2187, 2268 - 2187);
            Value = "" + Crypto.Converter.GetInt(ArrayUtils.SubArray(transactionTrits, 6804, 6837));
            Tag = trytes.Substring(2295, 2322 - 2295);
            Timestamp = "" + Crypto.Converter.GetInt(ArrayUtils.SubArray(transactionTrits, 6966, 6993));
            CurrentIndex = "" + Crypto.Converter.GetInt(ArrayUtils.SubArray(transactionTrits, 6993, 7020));
            LastIndex = "" + Crypto.Converter.GetInt(ArrayUtils.SubArray(transactionTrits, 7020, 7047));
            Bundle = trytes.Substring(2349, 2430 - 2349);
            TrunkTransaction = trytes.Substring(2430, 2511 - 2430);
            BranchTransaction = trytes.Substring(2511, 2592 - 2511);
            Nonce = trytes.Substring(2592, 2673 - 2592);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionItem"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="timestamp">The timestamp.</param>
        public TransactionItem(string address, string value, string tag, string timestamp)
        {
            Address = address;
            Value = value;
            Tag = tag;
            Timestamp = timestamp;
        }

        private string tag;
        private string address;
        private string value;
        private string timestamp;
        private string signatureMessageChunk;
        private string digest;
        private string type;

        private string bundle;
        //private int index;
        private string trunkTransaction;
        private string branchTransaction;
        private string signatureFragment;
        private string lastIndex;
        private string currentIndex;
        private string nonce;
        private bool persistence;

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (value != this.tag)
                {
                    this.tag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value != this.type)
                {
                    this.type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the signature message chunk.
        /// </summary>
        /// <value>
        /// The signature message chunk.
        /// </value>
        public string SignatureMessageChunk
        {
            get
            {
                return signatureMessageChunk;
            }
            set
            {
                if (value != this.signatureMessageChunk)
                {
                    this.signatureMessageChunk = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the digest.
        /// </summary>
        /// <value>
        /// The digest.
        /// </value>
        public string Digest
        {
            get
            {
                return digest;
            }
            set
            {
                if (value != this.digest)
                {
                    this.digest = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                if (value != this.address)
                {
                    this.address = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                if (value != this.timestamp)
                {
                    this.timestamp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the bundle.
        /// </summary>
        /// <value>
        /// The bundle.
        /// </value>
        public string Bundle
        {
            get
            {
                return bundle;
            }
            set
            {
                if (value != this.bundle)
                {
                    this.bundle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        ///// <summary>
        ///// Gets or sets the index.
        ///// </summary>
        ///// <value>
        ///// The index.
        ///// </value>
        //public int Index
        //{
        //    get
        //    {
        //        return index;
        //    }
        //    set
        //    {
        //        if (value != this.index)
        //        {
        //            this.index = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        /// <summary>
        /// Gets or sets the trunk transaction.
        /// </summary>
        /// <value>
        /// The trunk transaction.
        /// </value>
        public string TrunkTransaction
        {
            get
            {
                return trunkTransaction;
            }
            set
            {
                if (value != this.trunkTransaction)
                {
                    this.trunkTransaction = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the branch transaction.
        /// </summary>
        /// <value>
        /// The branch transaction.
        /// </value>
        public string BranchTransaction
        {
            get
            {
                return branchTransaction;
            }
            set
            {
                if (value != this.branchTransaction)
                {
                    this.branchTransaction = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the signature fragment.
        /// </summary>
        /// <value>
        /// The signature fragment.
        /// </value>
        public string SignatureFragment
        {
            get
            {
                return signatureFragment;
            }
            set
            {
                if (value != this.signatureFragment)
                {
                    this.signatureFragment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the last index.
        /// </summary>
        /// <value>
        /// The last index.
        /// </value>
        public string LastIndex
        {
            get
            {
                return lastIndex;
            }
            set
            {
                if (value != this.lastIndex)
                {
                    this.lastIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the current.
        /// </summary>
        /// <value>
        /// The index of the current.
        /// </value>
        public string CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                if (value != this.currentIndex)
                {
                    this.currentIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        /// <value>
        /// The nonce.
        /// </value>
        public string Nonce
        {
            get
            {
                return nonce;
            }
            set
            {
                if (value != this.nonce)
                {
                    this.nonce = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TransactionItem"/> is persistance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persistance; otherwise, <c>false</c>.
        /// </value>
        public bool Persistence
        {
            get
            {
                return persistence;
            }
            set
            {
                if (value != this.persistence)
                {
                    this.persistence = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Converts the transaction to the corresponding trytes representation
        /// </summary>
        /// <returns></returns>
        public string ToTransactionTrytes()
        {
            var valueTrits = Crypto.Converter.GetTrits(Value).ToLength(81);
            var timestampTrits = Crypto.Converter.GetTrits(Timestamp).ToLength(27);
            var currentIndexTrits = Crypto.Converter.GetTrits(CurrentIndex).ToLength(27);
            var lastIndexTrits = Crypto.Converter.GetTrits(LastIndex).ToLength(27);

            return SignatureFragment
                   + Address
                   + Crypto.Converter.GetTrytes(valueTrits)
                   + Tag
                   + Crypto.Converter.GetTrytes(timestampTrits)
                   + Crypto.Converter.GetTrytes(currentIndexTrits)
                   + Crypto.Converter.GetTrytes(lastIndexTrits)
                   + Bundle
                   + TrunkTransaction
                   + BranchTransaction
                   + Nonce;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}, {nameof(Persistence)}: {Value}, {nameof(Tag)}: {Tag}, {nameof(Hash)}: {Hash}, {nameof(Type)}: {Type}, {nameof(SignatureMessageChunk)}: {SignatureMessageChunk}, {nameof(Digest)}: {Digest}, {nameof(Address)}: {Address}, {nameof(Timestamp)}: {Timestamp}, {nameof(Bundle)}: {Bundle}, {nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}, {nameof(SignatureFragment)}: {SignatureFragment}, {nameof(LastIndex)}: {LastIndex}, {nameof(CurrentIndex)}: {CurrentIndex}, {nameof(Nonce)}: {Nonce}";
        }
    }
}
