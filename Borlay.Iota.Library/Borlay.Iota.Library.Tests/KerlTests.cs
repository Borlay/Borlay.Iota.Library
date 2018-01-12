using Borlay.Iota.Library.Crypto;
using Borlay.Iota.Library.Crypto.Sha3;
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


        [TestMethod]
        public void KerlAbsorbSqueeze()
        {
            var input = "GYOMKVTSNHVJNCNFBBAH9AAMXLPLLLROQY99QN9DLSJUHDPBLCFFAIQXZA9BKMBJCYSFHFPXAHDWZFEIZ";
            var expected = "OXJCNFHUNAHWDLKKPELTBFUCVW9KLXKOGWERKTJXQMXTKFKNWNNXYD9DMJJABSEIONOSJTTEVKVDQEWTW";

            var trits = Converter.GetTrits(input);
            var kerl = new Kerl();
            kerl.Initialize();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new sbyte[Kerl.HASH_LENGTH];
            kerl.Squeeze(hashTrits, 0, Kerl.HASH_LENGTH);
            var hash = Converter.GetTrytes(hashTrits);
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        public void KerlAbsorbMultiSqueeze()
        {
            var input = "9MIDYNHBWMBCXVDEFOFWINXTERALUKYYPPHKP9JJFGJEIUY9MUDVNFZHMMWZUYUSWAIOWEVTHNWMHANBH";
            var expected = "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA";

            var trits = Converter.GetTrits(input);
            var kerl = new Kerl();
            kerl.Initialize();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new sbyte[Kerl.HASH_LENGTH * 2];
            kerl.Squeeze(hashTrits, 0, Kerl.HASH_LENGTH * 2);
            var hash = Converter.GetTrytes(hashTrits);
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        public void KerlMultiAbsorbMultiSqueeze()
        {
            var input = "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA";
            var expected = "LUCKQVACOGBFYSPPVSSOXJEKNSQQRQKPZC9NXFSMQNRQCGGUL9OHVVKBDSKEQEBKXRNUJSRXYVHJTXBPDWQGNSCDCBAIRHAQCOWZEBSNHIJIGPZQITIBJQ9LNTDIBTCQ9EUWKHFLGFUVGGUWJONK9GBCDUIMAYMMQX";

            var trits = Converter.GetTrits(input);
            var kerl = new Kerl();
            kerl.Initialize();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new sbyte[Kerl.HASH_LENGTH * 2];
            kerl.Squeeze(hashTrits, 0, Kerl.HASH_LENGTH * 2);
            var hash = Converter.GetTrytes(hashTrits);
            Assert.AreEqual(expected, hash);
        }

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
        public void KeccakDigestTest()
        {
            var ints = new uint[]
            {
                9, 96, 55, 9457, 5, 9, 5, 9, 5, 9, 5, 9
            };

            var bytes = Kerl.ConvertToByteArray(ints);

            var sha3Digest = new KeccakDigest(Kerl.BIT_HASH_LENGTH);
            sha3Digest.BlockUpdate(bytes, 0, bytes.Length);

            var output = new byte[48];
            var count = sha3Digest.DoFinal(output, 0);

            var result = Kerl.ConvertToInt32Array(output);

            Assert.AreEqual(48, count);
            Assert.AreEqual(-1783424869, result[0]);
            Assert.AreEqual(-1114296599, result[1]);
            Assert.AreEqual(883842121, result[2]);
            Assert.AreEqual(1128053166, result[3]);

            Assert.AreEqual(1941138521, result[4]);
            Assert.AreEqual(545983793, result[5]);
            Assert.AreEqual(1613000376, result[6]);
            Assert.AreEqual(1678837429, result[7]);

            Assert.AreEqual(-1049582418, result[8]);
            Assert.AreEqual(161709750, result[9]);
            Assert.AreEqual(-308485616, result[10]);
            Assert.AreEqual(-139697445, result[11]);
        }

        [TestMethod]
        public void ConvertIntToBytes()
        {
            var ints = new uint[]
            {
                9, 5 
            };

            var bytes = Kerl.ConvertToByteArray(ints);

            Assert.AreEqual((byte)9, bytes[0]);
            Assert.AreEqual((byte)0, bytes[1]);
            Assert.AreEqual((byte)0, bytes[2]);
            Assert.AreEqual((byte)0, bytes[3]);

            Assert.AreEqual((byte)5, bytes[4]);
        }

        [TestMethod]
        public void ConvertBytesToInt()
        {
            var bytes = new byte[]
            {
                0, 0, 0, 9, 0, 0, 0, 5
            };

            var ints = Kerl.ConvertToInt32Array(bytes);

            Assert.AreEqual((int)9, ints[0]);
            Assert.AreEqual((int)5, ints[1]);
        }
    }
}
