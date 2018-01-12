using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    public class Converter
    {
        public const int RADIX = 3;
        public const int RADIX_BYTES = 256;
        public const int MAX_TRIT_VALUE = 1;
        public const int MIN_TRIT_VALUE = -1;
        public const int BYTE_HASH_LENGTH = 48;

        /// <summary>
        /// All possible tryte values
        /// </summary>
        public static string trytesAlphabet = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static sbyte[,] trytesTrits = new sbyte[,] {
            {0, 0, 0}, {1, 0, 0}, {-1, 1, 0}, {0, 1, 0}, {1, 1, 0}, {-1, -1, 1},
            {0, -1, 1}, {1, -1, 1}, {-1, 0, 1}, {0, 0, 1}, {1, 0, 1}, {-1, 1, 1},
            {0, 1, 1}, {1, 1, 1}, {-1, -1, -1}, {0, -1, -1}, {1, -1, -1}, {-1, 0, -1},
            {0, 0, -1}, {1, 0, -1}, {-1, 1, -1}, {0, 1, -1}, {1, 1, -1}, {-1, -1, 0},
            {0, -1, 0}, {1, -1, 0}, {-1, 0, 0} };

        /// <summary>
        /// Converts trytes into trits.
        /// </summary>
        /// <param name="trytes">The trytes. Can be string value or int</param>
        /// <param name="state">The state. Optional.</param>
        /// <returns></returns>
        public static sbyte[] GetTrits(object trytes, sbyte[] state = null)
        {
            if (trytes == null)
                throw new ArgumentNullException(nameof(trytes));

            var trits = state?.ToList() ?? new List<sbyte>();

            if (!(trytes is string))
            {
                var intValue = (int)trytes;
                return GetTritsFromInt(intValue);
            }
            else
            {
                var stringValue = (trytes as string) ?? trytes.ToString();

                for (var i = 0; i < stringValue.Length; i++)
                {
                    var index = trytesAlphabet.IndexOf(stringValue[i]);
                    trits.Add(trytesTrits[index, 0]);
                    trits.Add(trytesTrits[index, 1]);
                    trits.Add(trytesTrits[index, 2]);
                }
            }

            return trits.ToArray();
        }

        /// <summary>
        /// Get trytes from int value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static sbyte[] GetTritsFromInt(int value)
        {
            var destination = new List<sbyte>();
            var absoluteValue = Math.Abs(value);
            var i = 0;

            while (absoluteValue > 0)
            {

                var remainder = (sbyte)(absoluteValue % RADIX);
                absoluteValue = (int)Math.Floor((double)absoluteValue / RADIX);

                if (remainder > MAX_TRIT_VALUE)
                {
                    remainder = MIN_TRIT_VALUE;
                    absoluteValue++;
                }

                destination.Add(remainder);
                i++;

            }

            if (value < 0)
            {
                for (var j = 0; j < destination.Count; j++)
                {
                    // switch values
                    destination[j] = (sbyte)(-destination[j]);

                }
            }

            return destination.ToArray();
        }

        /// <summary>
        /// Get trytes from trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <returns></returns>
        public static string GetTrytes(sbyte[] trits)
        {
            var trytes = "";

            for (var i = 0; i < trits.Length; i += 3)
            {
                // Iterate over all possible tryte values to find correct trit representation
                for (var j = 0; j < trytesAlphabet.Length; j++)
                {
                    if (trytesTrits[j, 0] == trits[i] && trytesTrits[j, 1] == trits[i + 1] && trytesTrits[j, 2] == trits[i + 2])
                    {
                        trytes += trytesAlphabet[j];
                        break;

                    }
                }
            }

            return trytes;
        }

        /// <summary>
        /// Get int value from trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <returns></returns>
        public static int GetInt(sbyte[] trits)
        {
            var returnValue = 0;

            for (var i = trits.Length; i-- > 0;)
            {
                returnValue = returnValue * 3 + trits[i];
            }

            return returnValue;
        }

    }
}

//module.exports = {
//    trits           : trits,
//    trytes          : trytes,
//    value           : value,
//    fromValue       : fromValue
//};
