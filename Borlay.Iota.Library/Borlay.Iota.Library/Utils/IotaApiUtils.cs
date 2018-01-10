using Borlay.Iota.Library.Exceptions;
using Borlay.Iota.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Borlay.Iota.Library.Utils
{
    public static class IotaApiUtils
    {

        public static string GenerateRandomTrytes(int length = 81)
        {
            Random rand = new Random();
            var trytes = "";
            for(int i = 0; i < length; i++)
            {
                var index = rand.Next(27);
                trytes += Constants.TryteAlphabet[index];
            }
            return trytes;
        }

        /// <summary>
        ///  Generates a new address
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="index"></param>
        /// <param name="checksum"></param>
        /// <param name="curl"></param>
        /// <returns></returns>
        public static string NewAddress(int[] privateKey, bool checksum, ICurl curl, CancellationToken cancellationToken)
        {
            Signing signing = new Signing(curl);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            ts = stopWatch.Elapsed;
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            cancellationToken.ThrowIfCancellationRequested();

            int[] digests = signing.Digests(privateKey);

            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("After Digest {0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            cancellationToken.ThrowIfCancellationRequested();

            int[] addressTrits = signing.Address(digests);
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            cancellationToken.ThrowIfCancellationRequested();

            string address = Converter.ToTrytes(addressTrits);

            if (checksum)
                address = Checksum.AddChecksum(address);

            return address;
        }

        public static void SignSignatures(this IEnumerable<TransactionItem> transactionItems, IEnumerable<AddressItem> addressItems)
        {
            var curl = new Curl();
            foreach(var transactionItem in transactionItems)
            {
                var addressItem = addressItems.FirstOrDefault(a => a.Address == transactionItem.Address);
                if(addressItem != null)
                    transactionItem.SignSignature(addressItem.PrivateKey, curl);
            }
        }

        public static void SignSignature(this TransactionItem transactionItem, int[] addressPrivateKey, ICurl curl)
        {
            var value = Int64.Parse(transactionItem.Value);
            if (value > 0)
                return;
                //throw new IotaException($"Cannot sign transaction with value greater than 0. Current value '{value}'");

            int index = value < 0 ? 0 : 1;

            var normalizedBundleHash = TransactionExtensions.NormalizedBundle(transactionItem.Bundle);

            //  First 6561 trits for the firstFragment
            int[] firstFragment = ArrayUtils.SubArray2(addressPrivateKey, 6561 * index, 6561);
            //  First bundle fragment uses 27 trytes
            int[] firstBundleFragment = ArrayUtils.SubArray2(normalizedBundleHash, 27 * index, 27);
            //  Calculate the new signatureFragment with the first bundle fragment
            int[] firstSignedFragment = new Signing(curl).SignatureFragment(firstBundleFragment, firstFragment);

            //  Convert signature to trytes and assign the new signatureFragment
            transactionItem.SignatureFragment = Converter.ToTrytes(firstSignedFragment);
        }

        /// <summary>
        /// Create a unix date time by using the number of seconds from 01/01/1970.
        /// </summary>
        /// <returns></returns>
        internal static long CreateTimeStampNow()
        {
            var timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return timestamp; // 1515510395;
        }

        /// <summary>
        /// Calculates the number of milliseconds from 01/01/2017
        /// This is used for the attachment time where as the timestamp is a unix style and should use <see cref="CreateTimeStampNow"/>
        /// </summary>
        /// <returns></returns>
        internal static long CreateAttachmentTimeStampNow()
        {
            var timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return timestamp; // 1499592594121;
        }
    }
}
