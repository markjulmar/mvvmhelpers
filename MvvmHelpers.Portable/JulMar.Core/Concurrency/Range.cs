using System;
using System.Collections.Generic;

namespace JulMar.Concurrency
{
    /// <summary>
    /// Range structure for breaking a chunk into a range of
    /// values.  This is useful when trying to generate inner loops
    /// for Parallel.ForEach processing.
    /// </summary>
    public struct Range
    {
        /// <summary>
        /// Start of range
        /// </summary>
        public int Start;

        /// <summary>
        /// End of range (inclusive)
        /// </summary>
        public int End;

        /// <summary>
        /// Method to create a sub range from the current range.
        /// </summary>
        /// <param name="nRanges"># of blocks to break this range into</param>
        /// <returns>New IEnumerable range</returns>
        public IEnumerable<Range> CreateSubRanges(int nRanges)
        {
            if (nRanges < 1)
            {
                throw new ArgumentException("Number of Ranges must be greater than zero");
            }

            int subRangeStart = Start;
            int subRangeStep = ((End - Start)+1) / nRanges;
            while (nRanges > 1)
            {
                yield return new Range { Start = subRangeStart,  End = subRangeStart + subRangeStep-1 };
                subRangeStart += subRangeStep;
                nRanges--;
            }

            if (subRangeStart != End)
            {
                yield return new Range { Start = subRangeStart, End = End };
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} - {1}", Start, End);
        }
    }
}
