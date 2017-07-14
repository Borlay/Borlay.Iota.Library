using Borlay.Iota.Library.Iri.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Iri
{
    public interface IRequestErrorHandler
    {
        /// <summary>
        /// Gets or sets RetryCount. It can't reach GenericWebClient.MaximumRetryCount
        /// </summary>
        int RetryCount { get; set; }

        Task<bool> HandleError(object request, Exception exception, int tryCount, CancellationToken cancellationToken);
    }

    public class RequestErrorHandler : IRequestErrorHandler
    {
        /// <summary>
        /// Gets or sets RetryCount. It can't reach GenericWebClient.MaximumRetryCount
        /// </summary>
        public int RetryCount { get; set; }

        public RequestErrorHandler(int retryCount = 3)
        {
            this.RetryCount = retryCount;
        }

        public virtual async Task<bool> HandleError(object request, Exception exception, int tryCount, CancellationToken cancellationToken)
        {
            if (request is AttachToTangleRequest)
                return false;

            if (tryCount <= RetryCount)
                return true;

            return false;
        }
    }
}
