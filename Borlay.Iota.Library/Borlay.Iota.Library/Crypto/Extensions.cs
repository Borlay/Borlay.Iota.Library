using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    public static class Extensions
    {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        public static void TaReverse<T>(this T[] array)
        {

            var i = 0;
            var n = array.Length;
            var middle = Math.Floor((double)n / 2);
            T temp;

            for (; i < middle; i += 1)
            {
                temp = array[i];
                array[i] = array[n - 1 - i];
                array[n - 1 - i] = temp;
            }
        }
    }
}
