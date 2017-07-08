using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Exceptions
{
    public class IotaException : Exception
    {
        public IotaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IotaException(string message)
            : base(message)
        {
        }
    }
}
