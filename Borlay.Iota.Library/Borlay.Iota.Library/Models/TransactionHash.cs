using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    public class TransactionHash : ModelBase
    {
        private string hash;

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        public string Hash
        {
            get
            {
                return hash;
            }
            set
            {
                if (value != this.hash)
                {
                    this.hash = value;
                    NotifyPropertyChanged();
                }
            }
        }
            
    }
}
