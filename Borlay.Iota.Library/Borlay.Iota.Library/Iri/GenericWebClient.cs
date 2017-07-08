using Borlay.Iota.Library.Iri.Dto;
using Borlay.Iota.Library.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Iri
{
    public class GenericWebClient : IGenericWebClient
    {
        /// <summary>
        /// Gets or sets the url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets request log delegate
        /// </summary>
        public Action<string> Log { get; set; }

        public GenericWebClient(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            this.Url = url;
        }

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        public async Task<TResponse> RequestAsync<TResponse>(object request, CancellationToken cancellationToken) where TResponse : new()
        {
            string stringData = request.ToJson();
            Log?.Invoke($"Request: {stringData}");
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                client.DefaultRequestHeaders.Add("User-Agent", "borlay.iota.netcore");

                try
                {
                    var postResponse = await client.PostAsync(Url, contentData, cancellationToken);
                    var stringResult = await postResponse.Content.ReadAsStringAsync();

                    Log?.Invoke($"Request success: {postResponse.IsSuccessStatusCode}");
                    Log?.Invoke($"Response: {stringResult}");

                    if (postResponse.IsSuccessStatusCode)
                    {
                        var objectResult = JsonConvert.DeserializeObject<TResponse>(stringResult);
                        return objectResult;
                    }
                    else
                    {
                        var objectResult = JsonConvert.DeserializeObject<IriErrorResponse>(stringResult);
                        throw new IotaWebException(objectResult.Error);
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new IotaWebException(ex.Message, ex);
                    //using (var stream = ex.Response.GetResponseStream())
                    //using (var reader = new StreamReader(stream))
                    //{
                    //    String errorResponse = reader.ReadToEnd();
                    //    throw new IotaWebException(JsonConvert.DeserializeObject<IriErrorResponse>(errorResponse).Error, ex);
                    //}
                }
            }
        }
    }
}
