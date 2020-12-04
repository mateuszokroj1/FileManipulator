using System;
using System.Collections;
using System.Text;

namespace FileManipulator
{
    public class AlphanumericComparer : IComparer
    {
        private enum ChunkType { Alphanumeric, Numeric };

        private bool InChunk(char ch, char otherCh)
        {
            ChunkType type = ChunkType.Alphanumeric;

            if (char.IsDigit(otherCh))
            {
                type = ChunkType.Numeric;
            }

            return (type != ChunkType.Alphanumeric || !char.IsDigit(ch))
                && (type != ChunkType.Numeric || char.IsDigit(ch));
        }

        public int Compare(object x, object y)
        {
            if (!(x is string s1) || !(y is string s2))
            {
                return 0;
            }

            int thisMarker = 0, thisNumericChunk;
            int thatMarker = 0, thatNumericChunk;

            while (thisMarker < s1.Length || thatMarker < s2.Length)
            {
                if (thisMarker >= s1.Length)
                {
                    return -1;
                }
                else if (thatMarker >= s2.Length)
                {
                    return 1;
                }

                char thisCh = s1[thisMarker];
                char thatCh = s2[thatMarker];

                var thisChunk = new StringBuilder();
                var thatChunk = new StringBuilder();

                while (thisMarker < s1.Length && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < s1.Length)
                    {
                        thisCh = s1[thisMarker];
                    }
                }

                while (thatMarker < s2.Length && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < s2.Length)
                    {
                        thatCh = s2[thatMarker];
                    }
                }

                int result = 0;

                if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
                {
                    thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                    thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

                    if (thisNumericChunk < thatNumericChunk)
                    {
                        result = -1;
                    }

                    if (thisNumericChunk > thatNumericChunk)
                    {
                        result = 1;
                    }
                }
                else
                {
                    result = thisChunk.ToString().CompareTo(thatChunk.ToString());
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
}
