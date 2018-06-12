using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    /// <summary>
    /// This interface abstracts the curl hashing algorithm
    /// </summary>
    public interface ICurl
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        ICurl Clone();

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Absorb(sbyte[] trits, int offset, int length);

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Absorb(sbyte[] trits);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the squeezed trits</returns>
        sbyte[] Squeeze(sbyte[] trits, int offset, int length);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the squeezed trits</returns>
        sbyte[] Squeeze(sbyte[] trits);

        /// <summary>
        /// Transforms this instance.
        /// </summary>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Transform();

        /// <summary>
        /// Resets this state.
        /// </summary>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Reset();

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        sbyte[] State { get; }
    }
}
