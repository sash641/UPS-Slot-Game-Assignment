using System;
using System.Threading;

namespace JackpotSlots.Core
{
    /// <summary>
    /// Owns a private PRNG instead of tying game outcomes to animation timing or Unity's global random state.
    /// A unique seed per session makes repeated launches independent while keeping the API testable.
    /// </summary>
    public sealed class SlotRandom
    {
        private static int seedCounter;
        private readonly Random random;

        public SlotRandom()
            : this(Guid.NewGuid().GetHashCode() ^ Environment.TickCount ^ Interlocked.Increment(ref seedCounter))
        {
        }

        internal SlotRandom(int seed)
        {
            random = new Random(seed);
        }


        public int Next(int exclusiveMax) => random.Next(0, exclusiveMax);
    }
}
