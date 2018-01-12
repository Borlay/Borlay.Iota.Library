using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    public class Adder
    {

        public static sbyte Sum(sbyte a, sbyte b)
        {
            var s = a + b;

            switch (s)
            {

                case 2: return -1;
                case -2: return 1;
                default: return (sbyte)s;

            }
        }

        public static sbyte Cons(sbyte a, sbyte b)
        {
            if (a == b)
                return a;

            return 0;
        }

        public static sbyte Any(sbyte a, sbyte b)
        {
            var s = a + b;

            if (s > 0)
                return 1;
            else if (s < 0)
                return -1;

            return 0;
        }

        public static sbyte[] FullAdd(sbyte a, sbyte b, sbyte c)
        {
            var s_a = Sum(a, b);
            var c_a = Cons(a, b);
            var c_b = Cons(s_a, c);
            var c_out = Any(c_a, c_b);
            var s_out = Sum(s_a, c);

            return new sbyte[] { s_out, c_out };
        }

        public static sbyte[] Add(sbyte[] a, sbyte[] b)
        {
            var result = new sbyte[Math.Max(a.Length, b.Length)];
            sbyte carry = 0;
            sbyte a_i, b_i;

            for (var i = 0; i < result.Length; i++)
            {
                a_i = i < a.Length ? a[i] : (sbyte)0;
                b_i = i < b.Length ? b[i] : (sbyte)0;
                var f_a = FullAdd(a_i, b_i, carry);
                result[i] = f_a[0];
                carry = f_a[1];
            }

            return result;
        }
    }
}
