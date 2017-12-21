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
        public void Test()
        {
            var trits = Converter.GetTrits(input);
            var kerl = new Kerl();
            kerl.Initialize();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new sbyte[Curl.HASH_LENGTH];
            kerl.Squeeze(hashTrits, 0, Curl.HASH_LENGTH);
            var hash = Converter.GetTrytes(hashTrits);
            Assert.AreEqual(expected, hash);
            //assert.deepEqual(test.expected, hash);
        }
    }
}
