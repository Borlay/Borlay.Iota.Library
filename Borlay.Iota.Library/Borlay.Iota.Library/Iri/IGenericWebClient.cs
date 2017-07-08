using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Iri
{
    /// <summary>
    /// This interface abstracts a generic version of the core api that is used internally.
    /// </summary>
    public interface IGenericWebClient
    {
        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        string Url { get; }

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        Task<TResponse> RequestAsync<TResponse>(object request, CancellationToken cancellationToken)
            where TResponse : new();
    }
}
