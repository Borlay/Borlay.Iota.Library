using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Exceptions
{
    public class IotaWebException : IotaException
    {
        public IotaWebException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IotaWebException(string message)
            : base(message)
        {
        }
    }
}
