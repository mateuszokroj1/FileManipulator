using System;
using System.IO;

namespace FileManipulator
{
    public class TextFileIdentifier : ICheck
    {
        #region Constructor

        public TextFileIdentifier(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new IOException("Stream is not readable.");

            Reader = new BinaryReader(stream);
        }

        public TextFileIdentifier(BinaryReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        #endregion

        #region Properties

        public BinaryReader Reader { get; }

        #endregion

        public bool Check()
        {
            try
            {
                Reader.BaseStream.Position = 0;
            }
            catch(Exception) { return false; }

            int c;
            while((c = Reader.Read()) != -1)
            {
                switch(c)
                {
                    case 0:
                    case 8:
                    case 13:
                    case 26:
                        return false;
                }
            }

            return true;
        }
    }
}
