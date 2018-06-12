using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Borlay.Iota.Library.Models;

namespace Borlay.Iota.Library.Crypto
{
    public class Signing
    {
        public sbyte[] Key(sbyte[] seed, int index, int length)
        {
            if ((seed.Length % 243) != 0)
            {
                var listSeed = seed.ToList();
                while ((listSeed.Count % 243) != 0)
                {
                    listSeed.Add(0);
                }
                seed = listSeed.ToArray();
            }

            var indexTrits = Converter.GetTritsFromInt(index);
            var subseed = Adder.Add(seed, indexTrits);

            var kerl = new Kerl();

            kerl.Initialize();
            kerl.Absorb(subseed, 0, subseed.Length);
            kerl.Squeeze(subseed, 0, subseed.Length);

            kerl.Reset();
            kerl.Absorb(subseed, 0, subseed.Length);

            IList<sbyte> key = new List<sbyte>();
            sbyte[] buffer = new sbyte[subseed.Length];
            int offset = 0;

            while (length-- > 0)
            {
                for (int i = 0; i < 27; i++)
                {
                    kerl.Squeeze(buffer, offset, buffer.Length);
                    for (int j = 0; j < 243; j++)
                    {
                        key.Add(buffer[j]);
                    }
                }
            }
            return key.ToArray();
        }

        public sbyte[] Digests(sbyte[] key)
        {
            var fragments = (int)Math.Floor((decimal)(key.Length / 6561));
            var digests = new sbyte[fragments * 243];
            sbyte[] buffer = null;

            for (var i = 0; i < fragments; i++)
            {

                var keyFragment = key.Slice(i * 6561, (i + 1) * 6561);

                for (var j = 0; j < 27; j++)
                {

                    buffer = keyFragment.Slice(j * 243, (j + 1) * 243);

                    for (var k = 0; k < 26; k++)
                    {

                        var kKerl = new Kerl();
                        kKerl.Initialize();
                        kKerl.Absorb(buffer, 0, buffer.Length);
                        kKerl.Squeeze(buffer, 0, Curl.HASH_LENGTH);
                    }

                    for (var k = 0; k < 243; k++)
                    {

                        keyFragment[j * 243 + k] = buffer[k];
                    }
                }

                var kerl = new Kerl();

                kerl.Initialize();
                kerl.Absorb(keyFragment, 0, keyFragment.Length);
                kerl.Squeeze(buffer, 0, Curl.HASH_LENGTH);

                for (var j = 0; j < 243; j++)
                {
                    digests[i * 243 + j] = buffer[j];
                }
            }

            return digests;
        }

        public sbyte[] Address(sbyte[] digests)
        {
            var addressTrits = new sbyte[Curl.HASH_LENGTH];

            var kerl = new Kerl();

            kerl.Initialize();
            kerl.Absorb(digests, 0, digests.Length);
            kerl.Squeeze(addressTrits, 0, Curl.HASH_LENGTH);

            return addressTrits;
        }

        public sbyte[] Digest(int[] normalizedBundleFragment, sbyte[] signatureFragment)
        {
            sbyte[] buffer = null;
            var kerl = new Kerl();
            kerl.Initialize();
            for(var i = 0; i < 27; i++)
            {
                buffer = signatureFragment.Slice(i * 243, (i + 1) * 243);
                for(var j = normalizedBundleFragment[i] + 13; j-- > 0;)
                {
                    var jKerl = new Kerl();
                    jKerl.Initialize();
                    jKerl.Absorb(buffer, 0, buffer.Length);
                    jKerl.Squeeze(buffer, 0, Curl.HASH_LENGTH);
                }
                kerl.Absorb(buffer, 0, buffer.Length);
            }

            kerl.Squeeze(buffer, 0, Curl.HASH_LENGTH);
            return buffer;
        }

        public sbyte[] SignatureFragment(int[] normalizedBundleFragment, sbyte[] keyFragment)
        {
            var signatureFragment = keyFragment.ToArray();
            var hash = new sbyte[0];
            var kerl = new Kerl();

            for (var i = 0; i < 27; i++)
            {
                hash = signatureFragment.Slice(i * 243, (i + 1) * 243);

                for (var j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    kerl.Initialize();
                    kerl.Reset();
                    kerl.Absorb(hash, 0, hash.Length);
                    kerl.Squeeze(hash, 0, Curl.HASH_LENGTH);
                }

                for (var j = 0; j < 243; j++)
                {
                    signatureFragment[i * 243 + j] = hash[j];
                }
            }

            return signatureFragment;

        }

        public bool ValidateSignatures(string expectedAddress, sbyte[] signatureFragments, string bundleHash)
        {
            if (string.IsNullOrWhiteSpace(bundleHash))
                throw new ArgumentNullException(nameof(bundleHash));

            var bundle = new Bundle();

            var normalizedBundleFragments = new List<int[]>(3);
            var normalizedBundleHash = TransactionExtensions.NormalizedBundle(bundleHash);

            // Split hash into 3 fragments
            for (var i = 0; i < 3; i++)
            {
                normalizedBundleFragments[i] = normalizedBundleHash.Slice(i * 27, (i + 1) * 27);
            }

            // Get digests
            var digests = new sbyte[signatureFragments.Length * 243];

            for (var i = 0; i < signatureFragments.Length; i++)
            {

                var digestBuffer = Digest(normalizedBundleFragments[i % 3], Converter.GetTrits(signatureFragments[i]));

                for (var j = 0; j < 243; j++)
                {
                    digests[i * 243 + j] = digestBuffer[j];
                }
            }

            var address = Converter.GetTrytes(Address(digests));

            return (expectedAddress == address);
        }
    }
}
