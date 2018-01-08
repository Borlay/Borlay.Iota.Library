using Borlay.Iota.Library.Crypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Tests
{
    [TestClass]
    public class KerlTests
    {
        private string input = "GYOMKVTSNHVJNCNFBBAH9AAMXLPLLLROQY99QN9DLSJUHDPBLCFFAIQXZA9BKMBJCYSFHFPXAHDWZFEIZ";
    private string expected = "OXJCNFHUNAHWDLKKPELTBFUCVW9KLXKOGWERKTJXQMXTKFKNWNNXYD9DMJJABSEIONOSJTTEVKVDQEWTW";

        [TestMethod]
        public void WordsToTritsAndBack()
        {
            var ints = new int[]
            {
                9, 9, 5, 9, 5, 9, 5, 9, 5, 9, 5, 9
            };
            var trits =  Words.words_to_trits(ints);

            var bints = Words.trits_to_words(trits);

            for(int i = 0; i < ints.Length; i++)
            {
                var swapBack = Words.swap32(bints[i]);
                Assert.AreEqual(ints[i], (int)swapBack);
            }
        }

        [TestMethod]
        public void KerlHash()
        {
            var trits = Converter.GetTrits(input);
            var kerl = new Kerl();
            kerl.Initialize();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new sbyte[Curl.HASH_LENGTH];
            kerl.Squeeze(hashTrits, 0, Curl.HASH_LENGTH);
            var hash = Converter.GetTrytes(hashTrits);
            Assert.AreEqual(expected, hash);
        }
    }
}
