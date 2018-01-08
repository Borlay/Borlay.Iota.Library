using System;
using System.Collections.Generic;
using System.Text;
using Borlay.Iota.Library.Crypto.Sha3;
using System.Linq;

namespace Borlay.Iota.Library.Crypto
{
    public class Kerl
    {
        public const int BIT_HASH_LENGTH = 384;
        private Sha3Digest sha3Digest;

        public Kerl()
        {
            sha3Digest = new Sha3Digest(BIT_HASH_LENGTH);
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
                var limit = (length < Curl.HASH_LENGTH ? length : Curl.HASH_LENGTH);

                var trit_state = trits.Slice(offset, offset + limit);//.ToArray();
                offset += limit;

                // convert trit state to words
                var wordsToAbsorb = Words.trits_to_words(trit_state);


                var wordArray = ConvertIntArrayToByteArray(wordsToAbsorb);
                sha3Digest.BlockUpdate(wordArray, 0, wordArray.Length);

                // absorb the trit stat as wordarray
                //this.k.update(
                //    CryptoJS.lib.WordArray.create(wordsToAbsorb));

            } while ((length -= Curl.HASH_LENGTH) > 0);
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
                var kCopy = (Sha3Digest)sha3Digest.Copy();
                byte[] final = new byte[Curl.HASH_LENGTH*4];
                var fLen = kCopy.DoFinal(final, 0);
                final = final.Take(fLen).ToArray();

                // Convert words to trits and then map it into the internal state
                var trit_state = Words.words_to_trits(ConvertToInt32Array(final));

                var i = 0;
                var limit = (length < Curl.HASH_LENGTH ? length : Curl.HASH_LENGTH);

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

            } while ((length -= Curl.HASH_LENGTH) > 0);
        }


        public byte[] ConvertIntArrayToByteArray(uint[] inputElements)
        {
            var len = 4;
            byte[] myFinalBytes = new byte[inputElements.Length * len];
            for (int cnt = 0; cnt < inputElements.Length; cnt++)
            {
                byte[] myBytes = BitConverter.GetBytes(inputElements[cnt]).ToArray();
                Array.Copy(myBytes, 0, myFinalBytes, cnt * len, len);
            }
            return myFinalBytes;
        }

        public int[] ConvertToInt32Array(byte[] inputElements)
        {
            var len = 4;
            int[] myFinalIntegerArray = new int[inputElements.Length / len];
            for (int cnt = 0; cnt < inputElements.Length; cnt += len)
            {
                myFinalIntegerArray[cnt / len] = BitConverter.ToInt32(inputElements.ToArray(), cnt);
            }
            return myFinalIntegerArray;
        }

    }
}

/*
Kerl.prototype.absorb = function(trits, offset, length)
{


    if (length && ((length % 243) !== 0))
    {

        throw new Error('Illegal length provided');

    }

    do
    {
        var limit = (length < Curl.HASH_LENGTH ? length : Curl.HASH_LENGTH);

        var trit_state = trits.slice(offset, offset + limit);
        offset += limit;

        // convert trit state to words
        var wordsToAbsorb = WConverter.trits_to_words(trit_state);

        // absorb the trit stat as wordarray
        this.k.update(
            CryptoJS.lib.WordArray.create(wordsToAbsorb));

    } while ((length -= Curl.HASH_LENGTH) > 0);

}



Kerl.prototype.squeeze = function(trits, offset, length)
{

    if (length && ((length % 243) !== 0))
    {

        throw new Error('Illegal length provided');

    }
    do
    {

        // get the hash digest
        var kCopy = this.k.clone();
        var final = kCopy.finalize();

        // Convert words to trits and then map it into the internal state
        var trit_state = WConverter.words_to_trits(final.words);

        var i = 0;
        var limit = (length < Curl.HASH_LENGTH ? length : Curl.HASH_LENGTH);

        while (i < limit)
        {
            trits[offset++] = trit_state[i++];
        }

        this.reset();

        for (i = 0; i < final.words.length; i++)
        {
            final.words[i] = final.words[i] ^ 0xFFFFFFFF;
        }

        this.k.update(final);

    } while ((length -= Curl.HASH_LENGTH) > 0);
}
*/