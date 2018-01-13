using Borlay.Iota.Library.Crypto;
using Borlay.Iota.Library.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    public class AddressModelBase : ModelBase
    {
        private string address;
        private string addressWithCheckSum;

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
                return this.address;
            }
            set
            {
                if (value != this.address)
                {
                    this.address = value;
                    this.addressWithCheckSum = Checksum.AddChecksum(address);
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(AddressWithCheckSum));
                }
            }
        }

        public string AddressWithCheckSum
        {
            get
            {
                return this.addressWithCheckSum;
            }
        }
    }
}
