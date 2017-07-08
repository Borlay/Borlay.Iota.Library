using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Iota.Library
{
    public interface ITransactionHandler
    {
        Task<HandlerResult> FailedToAttachToTangleAsync(IotaApi api, string[] trytes, Exception exception);
        Task SuccessToAttachToTangleAsync(IotaApi api, string[] trytes);
        Task<HandlerResult> FailedToBroadcastAndStoreAsync(IotaApi api, string[] trytes, Exception exception);
        Task SuccessToBroadcastAndStoreAsync(IotaApi api, string[] trytes);
    }

    public enum HandlerResult
    {
        NotHandled = 0,
        Handled
    }
}
