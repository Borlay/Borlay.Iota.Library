using Borlay.Iota.Library.Crypto;
//using Borlay.Iota.Library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Borlay.Iota.Library.Models
{
    public static class TransactionExtensions
    {
        public static void FinalizeBundleHash(this IEnumerable<TransactionItem> transactionItems)
        {
            var validBundle = false;
            while (!validBundle)
            {
                var kerl = new Kerl();
                kerl.Reset();
                var transactionCount = transactionItems.Count();

                for (int i = 0; i < transactionCount; i++)
                {
                    var transaction = transactionItems.ElementAt(i);

                    var valueTrits = Converter.GetTrits(transaction.Value).ToLength(81);

                    var timestampTrits = Converter.GetTrits(transaction.Timestamp).ToLength(27);

                    var currentIndexTrits = Converter.GetTrits(transaction.CurrentIndex = ("" + i)).ToLength(27);

                    var lastIndexTrits = Converter.GetTrits(
                        transaction.LastIndex = ("" + (transactionCount - 1))).ToLength(27);

                    string stringToConvert = transaction.Address
                                             + Converter.GetTrytes(valueTrits)
                                             + transaction.Tag +
                                             Converter.GetTrytes(timestampTrits)
                                             + Converter.GetTrytes(currentIndexTrits) +
                                             Converter.GetTrytes(lastIndexTrits);

                    var t = Converter.GetTrits(stringToConvert);
                    kerl.Absorb(t, 0, t.Length);
                }

                sbyte[] hash = new sbyte[Curl.HASH_LENGTH];
                kerl.Squeeze(hash, 0, hash.Length);
                string hashInTrytes = Converter.GetTrytes(hash);

                foreach (var transaction in transactionItems)
                    transaction.Bundle = hashInTrytes;

                var normalizedHash = NormalizedBundle(hashInTrytes);
                if (normalizedHash.Contains(13))
                {
                    // Insecure bundle. Increment Tag and recompute bundle hash.
                    var firstTransaction = transactionItems.ElementAt(0);
                    var increasedTag = Adder.Add(Converter.GetTrits(firstTransaction.Tag), new sbyte[1]);
                    firstTransaction.Tag = Converter.GetTrytes(increasedTag);
                }
                else
                {
                    validBundle = true;
                }
            }
        }

        //public static int[] FinalizeAndNormalizeBundleHash(this IEnumerable<TransactionItem> transactionItems)
        //{
        //    var bundleHash = transactionItems.FinalizeBundleHash();
        //    return NormalizedBundle(bundleHash);
        //}

        /// <summary>
        /// Normalizeds the bundle.
        /// </summary>
        /// <param name="bundleHash">The bundle hash.</param>
        /// <returns></returns>
        public static int[] NormalizedBundle(string bundleHash)
        {
            int[] normalizedBundle = new int[81];

            for (int i = 0; i < 3; i++)
            {
                long sum = 0;
                for (int j = 0; j < 27; j++)
                {
                    sum +=
                    (normalizedBundle[i * 27 + j] =
                        (int)Converter.GetInt(Converter.GetTrits("" + bundleHash[i * 27 + j])));
                }

                if (sum >= 0)
                {
                    while (sum-- > 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i * 27 + j] > -13)
                            {
                                normalizedBundle[i * 27 + j]--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (sum++ < 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i * 27 + j] < 13)
                            {
                                normalizedBundle[i * 27 + j]++;
                                break;
                            }
                        }
                    }
                }
            }

            return normalizedBundle;
        }
    }
}
