using Borlay.Iota.Library.Iri;
using Borlay.Iota.Library.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library
{
    public interface INonceSeeker
    {
        int MinWeightMagnitude { get; set; }

        Task<string[]> SearchNonce(string[] trytes, string trunkTransaction, string branchTransaction, CancellationToken cancellationToken);
    }

    public class LocalNonceSeeker : INonceSeeker
    {
        public int MinWeightMagnitude { get; set; }

        public int NumberOfThreads { get; set; }

        public LocalNonceSeeker(int minWeightMagnitude)
            : this(minWeightMagnitude, 0)
        {
        }

        public LocalNonceSeeker(int minWeightMagnitude, int numberOfThreads)
        {
            if (numberOfThreads <= 0)
            {
                numberOfThreads = Environment.ProcessorCount - 1;
                if (numberOfThreads < 1)
                {
                    numberOfThreads = 1;
                }
            }

            this.MinWeightMagnitude = minWeightMagnitude;
            this.NumberOfThreads = numberOfThreads;
        }

        public async Task<string[]> SearchNonce(string[] trytes, string trunkTransaction, string branchTransaction, CancellationToken cancellationToken)
        {
            var resultTrytes = await trytes.DoPow(trunkTransaction, branchTransaction, MinWeightMagnitude, NumberOfThreads, cancellationToken);
            return resultTrytes;
        }
    }
}
