using Borlay.Iota.Library.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Borlay.Iota.Library.Exceptions;
using System.Runtime.CompilerServices;

namespace Borlay.Iota.Library.Utils
{
    /// <summary>
    /// This class provides methods to validate the parameters of different iota API methods
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Determines whether the specified string is an adrdress.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is an address; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAddress(string address)
        {
            if (address.Length == Constants.AddressLengthWithoutChecksum ||
                address.Length == Constants.AddressLengthWithChecksum)
            {
                return IsTrytes(address, address.Length);
            }
            return false;
        }

        /// <summary>
        /// Checks whether the specified address is an address and throws and exception if the address is invalid
        /// </summary>
        /// <param name="address">address to check</param>
        /// <exception cref="InvalidAddressException">exception which is thrown when the address is invalid</exception>
        public static void CheckAddress(string address)
        {
            if (!IsAddress(address))
                throw new InvalidAddressException(address);
        }

        /// <summary>
        /// Determines whether the specified string represents an integer value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> the specified string represents an integer value; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValue(string value)
        {
            return Regex.IsMatch(value, @"^(-){0,1}\d+$");
        }

        /// <summary>
        /// Determines whether the specified array contains only valid hashes
        /// </summary>
        /// <param name="hashes">The hashes.</param>
        /// <returns>
        ///   <c>true</c> the specified array contains only valid hashes; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArrayOfHashes(IEnumerable<string> hashes)
        {
            if (hashes == null)
                return false;

            foreach (string hash in hashes)
            {
                // Check if address with checksum
                if (hash.Length == 90)
                {
                    if (!IsTrytes(hash, 90))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!IsTrytes(hash, 81))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified string contains only characters from the trytes alphabet (see <see cref="Constants.TryteAlphabet"/>)
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified trytes are trytes otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTrytes(string trytes, int length = 0)
        {
            string regex = "^[9A-Z]{" + (length == 0 ? "0," : length.ToString()) + "}$";
            var regexTrytes = new Regex(regex);
            return regexTrytes.IsMatch(trytes);
        }

        /// <summary>
        /// Determines whether the specified string array contains only trytes
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified array contains only valid trytes otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArrayOfTrytes(IEnumerable<string> trytes, int length)
        {
            return trytes.All(element => IsTrytes(element, length));
        }

        /// <summary>
        /// Determines whether the specified transfers are valid
        /// </summary>
        /// <param name="transfers">The transfers.</param>
        /// <returns>
        ///   <c>true</c> if the specified transfers are valid; otherwise, <c>false</c>.
        /// </returns>
        public static void ValidateTransfers(this IEnumerable<TransferItem> transfers)
        {
            foreach (var transfer in transfers)
            {
                transfer.ValidateTransfer();
            }
        }

        /// <summary>
        /// Determines whether the specified transfer is valid.
        /// </summary>
        /// <param name="transfer">The transfer.</param>
        public static void ValidateTransfer(this TransferItem transfer)
        {
            if (string.IsNullOrWhiteSpace(transfer.Address))
                throw new IotaException("Transfer address should be set");

            if (!IsAddress(transfer.Address))
                throw new IotaException("Invalid transfer address");

            transfer.Message.ValidateTrytes(nameof(transfer.Message));
            transfer.Tag.ValidateTrytes(nameof(transfer.Tag));
        }

        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        public static string ValidateTrytes(this string value, [CallerMemberName]string propertyName = "")
        {
            if (!string.IsNullOrEmpty(value))
            {
                // Check if value is correct trytes of any length
                if (!IsTrytes(value, 0))
                    throw new IotaException($"Invalid transfer {propertyName}");
            }
            return value;
        }

        /// <summary>
        /// Checks if the seed is valid. If not, an exception is thrown.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <exception cref="IllegalStateException">
        /// Invalid Seed: Format not in trytes
        /// or
        /// Invalid Seed: Seed too long
        /// </exception>
        public static void CheckIfValidSeed(string seed)
        {
            // validate the seed
            if (!IsTrytes(seed, 0))
            {
                throw new IllegalStateException("Invalid Seed: Format not in trytes");
            }

            // validate & if needed pad seed
            if (seed.Length > 81)
            {
                throw new IllegalStateException("Invalid Seed: Seed too long");
            }
        }

        /// <summary>
        /// Pads the seed if necessary.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public static string PadSeedIfNecessary(string seed)
        {
            while (seed.Length < Constants.AddressLengthWithoutChecksum) seed += 9;
            return seed;
        }

        /// <summary>
        /// Checks if the specified array is an array of trytes. If not an exception is thrown.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <exception cref="InvalidTryteException"></exception>
        public static void CheckIfArrayOfTrytes(IEnumerable<string> trytes)
        {
            if (!IsArrayOfTrytes(trytes, 2673))
                throw new InvalidTryteException();
        }

        /// <summary>
        /// Determines whether the specified string consist only of '9'.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified string consist only of '9'; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNinesTrytes(string trytes, int length)
        {
            return Regex.IsMatch(trytes, "^[9]{" + (length == 0 ? "0," : length.ToString()) + "}$");
            //return trytes.Matches("^[9]{" + (length == 0 ? "0," : length.ToString()) + "}$");
        }
    }
}
