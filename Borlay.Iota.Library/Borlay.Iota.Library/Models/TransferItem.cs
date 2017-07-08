using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    /// <summary>
    /// This class represents a Transfer
    /// </summary>
    public class TransferItem : AddressModelBase
    {
        private string hash;
        private int persistence;
        private string timestamp;

        private long value;
        private string message;
        private string tag;


        /// <summary>
        /// Initializes a new instance of the <see cref="TransferItem"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        public TransferItem() //(string address, long value, string message, string tag)
        {
            //this.Address = address;
            //this.Value = value;
            //this.Message = message;
            //this.Tag = tag;
        }

        ///// <summary>
        ///// Gets or sets the hash.
        ///// </summary>
        ///// <value>
        ///// The hash.
        ///// </value>
        //public string Hash
        //{
        //    get
        //    {
        //        return this.hash;
        //    }
        //    set
        //    {
        //        if (value != this.hash)
        //        {
        //            this.hash = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the persistence.
        ///// </summary>
        ///// <value>
        ///// The persistence.
        ///// </value>
        //public int Persistence
        //{
        //    get
        //    {
        //        return this.persistence;
        //    }
        //    set
        //    {
        //        if (value != this.persistence)
        //        {
        //            this.persistence = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the timestamp.
        ///// </summary>
        ///// <value>
        ///// The timestamp.
        ///// </value>
        //public string Timestamp
        //{
        //    get
        //    {
        //        return this.timestamp;
        //    }
        //    set
        //    {
        //        if (value != this.timestamp)
        //        {
        //            this.timestamp = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public long Value
        {
            get
            {
                return this.value;
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
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (value != this.message)
                {
                    this.message = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
                return this.tag;
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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(Message)}: {Message}, {nameof(Tag)}: {Tag}, {nameof(Value)}: {Value}";
        }
    }
}
