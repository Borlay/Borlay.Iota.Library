using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    public class Curl : ICurl
    {
        public const int NUMBER_OF_ROUNDS = 81;
        public const int HASH_LENGTH = 243;
        public const int STATE_LENGTH = 3 * HASH_LENGTH;

        private static int[] truthTable = new int[] { 1, 0, -1, 2, 1, -1, 0, 2, -1, 1, 0 };

        private int[] state = new int[STATE_LENGTH];
        private int rounds;
        

        public int[] State => state;

        public Curl()
            : this(NUMBER_OF_ROUNDS)
        {
        }


        public Curl(int rounds)
        {
            this.rounds = rounds;
        }

        public ICurl Initialize()
        {
            return Initialize(null);
        }

        public ICurl Initialize(int[] state)
        {
            if (state != null)
                this.state = state;
            else
            {
                for (var i = 0; i < state.Length; i++)
                {
                    this.state[i] = 0;
                }
            }
            return this;
        }

        public ICurl Reset()
        {
            return Initialize();
        }

        public ICurl Absorb(int[] trits, int offset, int length)
        {
            do
            {
                var i = 0;
                var limit = (length < HASH_LENGTH ? length : HASH_LENGTH);

                while (i < limit)
                {
                    this.state[i++] = trits[offset++];
                }

                this.Transform();

            } while ((length -= HASH_LENGTH) > 0);
            return this;
        }

        public int[] Squeeze(int[] trits, int offset, int length)
        {

            do
            {
                var i = 0;
                var limit = (length < HASH_LENGTH ? length : HASH_LENGTH);

                while (i < limit)
                {

                    trits[offset++] = this.state[i++];
                }

                this.Transform();

            } while ((length -= HASH_LENGTH) > 0);

            return trits;
        }

        public ICurl Transform()
        {
            var index = 0;

            for (var round = 0; round < this.rounds; round++)
            {
                var stateCopy = this.state.ToArray();

                for (var i = 0; i < STATE_LENGTH; i++)
                {
                    this.state[i] = truthTable[stateCopy[index] + (stateCopy[index += (index < 365 ? 364 : -365)] << 2) + 5];
                }
            }

            return this;
        }

        public ICurl Clone()
        {
            return new Curl();
        }

        public ICurl Absorb(int[] trits)
        {
            return Absorb(trits, 0, trits.Length);
        }

        public int[] Squeeze(int[] trits)
        {
            return Squeeze(trits, 0, trits.Length);
        }
    }
}
