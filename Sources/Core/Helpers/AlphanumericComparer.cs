using System;
using System.Collections.Generic;
using System.Text;

namespace FileManipulator
{
    public class AlphanumericComparer : IComparer<string>
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

        public int Compare(string x, string y)
        {
            int thisMarker = 0, thisNumericChunk;
            int thatMarker = 0, thatNumericChunk;

            while (thisMarker < x.Length || thatMarker < y.Length)
            {
                if (thisMarker >= x.Length)
                {
                    return -1;
                }
                else if (thatMarker >= y.Length)
                {
                    return 1;
                }

                char thisCh = x[thisMarker];
                char thatCh = y[thatMarker];

                var thisChunk = new StringBuilder();
                var thatChunk = new StringBuilder();

                while (thisMarker < x.Length && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < x.Length)
                    {
                        thisCh = x[thisMarker];
                    }
                }

                while (thatMarker < y.Length && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < y.Length)
                    {
                        thatCh = y[thatMarker];
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
