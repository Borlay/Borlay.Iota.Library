using Borlay.Iota.Library.Exceptions;
using Borlay.Iota.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Borlay.Iota.Library.Crypto;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Utils
{
    public static class IotaUtils
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
        /// Offline generates an address
        /// </summary>
        /// <param name="seed">The seed from which an address should be generated</param>
        /// <param name="index">The index of the address</param>
        /// <param name="security">Security level (1, 2, 3)</param>
        /// <param name="addChecksum">True to add checksum</param>
        /// <returns></returns>
        public static AddressItem GenerateAddress(string seed, int index, int security, bool addChecksum)
        {
            return GenerateAddress(seed, index, security, addChecksum, CancellationToken.None);
        }

        /// <summary>
        /// Offline generates an address
        /// </summary>
        /// <param name="seed">The seed from which an address should be generated</param>
        /// <param name="index">The index of the address</param>
        /// <param name="security">Security level (1, 2, 3)</param>
        /// <param name="addChecksum">True to add checksum</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public static AddressItem GenerateAddress(string seed, int index, int security, bool addChecksum, CancellationToken cancellationToken)
        {
            var trits = Crypto.Converter.GetTrits(seed);
            sbyte[] key = new Crypto.Signing().Key(trits, index, security);
            string address = GenerateAddress(key, addChecksum, cancellationToken);

            var addressItem = new AddressItem()
            {
                Address = address,
                PrivateKey = key,
                Index = index,
                Balance = 0
            };

            return addressItem;
        }

        /// <summary>
        ///  Generates an address from private key
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="index"></param>
        /// <param name="checksum"></param>
        /// <param name="curl"></param>
        /// <returns></returns>
        public static string GenerateAddress(sbyte[] privateKey, bool checksum, CancellationToken cancellationToken)
        {
            Crypto.Signing signing = new Crypto.Signing();
            var digests = signing.Digests(privateKey);

            cancellationToken.ThrowIfCancellationRequested();

            var addressTrits = signing.Address(digests);
            
            cancellationToken.ThrowIfCancellationRequested();

            string address = Crypto.Converter.GetTrytes(addressTrits);

            if (checksum)
                address = Checksum.AddChecksum(address);

            return address;
        }

        public static void SignSignatures(this IEnumerable<TransactionItem> transactionItems, IEnumerable<AddressItem> addressItems)
        {
            throw new NotImplementedException("SignSignatures");
            // todo need to implement
            //var curl = new Curl();
            //foreach(var transactionItem in transactionItems)
            //{
            //    var addressItem = addressItems.FirstOrDefault(a => a.Address == transactionItem.Address);
            //    if(addressItem != null)
            //        transactionItem.SignSignature(addressItem.PrivateKey, curl);
            //}
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
