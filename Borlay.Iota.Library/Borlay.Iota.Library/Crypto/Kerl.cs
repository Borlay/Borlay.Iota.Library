using System;
using System.Collections.Generic;
using System.Text;
using Borlay.Iota.Library.Crypto.Sha3;
using System.Linq;

namespace Borlay.Iota.Library.Crypto
{
    public class Kerl
    {
        public const int NUMBER_OF_ROUNDS = 81;
        public const int HASH_LENGTH = 243;
        //public const int STATE_LENGTH = 3 * HASH_LENGTH;

        public const int BIT_HASH_LENGTH = 384;
        private KeccakDigest sha3Digest;

        public Kerl()
        {
            sha3Digest = new KeccakDigest(BIT_HASH_LENGTH);
        }

        public void Initialize()
        {

        }

        public void Reset()
        {
            sha3Digest.Reset();
        }

        public void Absorb(sbyte[] trits, int offset, int length)
        {
            if (((length % 243) != 0))
            {
                throw new Exception("Illegal length provided");
            }

            do
            {
                var limit = (length < Kerl.HASH_LENGTH ? length : Kerl.HASH_LENGTH);

                var trit_state = trits.Slice(offset, offset + limit);//.ToArray();
                offset += limit;

                // convert trit state to words
                var wordsToAbsorb = Words.trits_to_words(trit_state);


                var wordArray = ConvertToByteArray(wordsToAbsorb);
                sha3Digest.BlockUpdate(wordArray, 0, wordArray.Length);

                // absorb the trit stat as wordarray
                //this.k.update(
                //    CryptoJS.lib.WordArray.create(wordsToAbsorb));

            } while ((length -= Kerl.HASH_LENGTH) > 0);
        }

        public void Squeeze(sbyte[] trits, int offset, int? length)
        {

            if (!length.HasValue)
                length = trits.Length;

            if (((length % 243) != 0))
            {
                throw new Exception("Illegal length provided");
            }
            do
            {

                // get the hash digest
                var kCopy = (KeccakDigest)sha3Digest.Copy();
                byte[] final = new byte[48];
                var fLen = kCopy.DoFinal(final, 0);

                // Convert words to trits and then map it into the internal state
                var trit_state = Words.words_to_trits(ConvertToInt32Array(final));

                var i = 0;
                var limit = (length < Kerl.HASH_LENGTH ? length : Kerl.HASH_LENGTH);

                while (i < limit)
                {
                    trits[offset++] = trit_state[i++];
                }

                sha3Digest.Reset();

                for (i = 0; i < final.Length; i++)
                {
                    final[i] = (byte)(final[i] ^ 0xFF); //(byte)(final[i] ^ 0xFFFFFFFF);
                }

                sha3Digest.BlockUpdate(final, 0, final.Length);

            } while ((length -= Kerl.HASH_LENGTH) > 0);
        }


        public static byte[] ConvertToByteArray(uint[] inputInts)
        {
            var len = 4;
            byte[] byteArray = new byte[inputInts.Length * len];

            for (int i = 0; i < inputInts.Length; i++)
            {
                var bytes = BitConverter.GetBytes(inputInts[i]);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                Array.Copy(bytes, 0, byteArray, i * len, len);

            }

            return byteArray;
        }

        public static int[] ConvertToInt32Array(byte[] inputBytes)
        {
            var len = 4;
            int[] intArray = new int[inputBytes.Length / len];

            for(int i = 0; i < intArray.Length; i++)
            {
                var bytes = new byte[4];
                Array.Copy(inputBytes, i * len, bytes, 0, len);
                var value = ConvertToInt32(bytes);
                intArray[i] = value;
            }

            return intArray;
        }

        public static int ConvertToInt32(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            int result = BitConverter.ToInt32(bytes, 0);
            return result;
        }

    }
}