using System.IO;

namespace FileManipulator.Models.Manipulator.FileInfos
{
    public class ISourceFileInfo : ISourceFileInfo
    {
        public string SourceFileName { get; internal set; }

        public StreamReader SourceFileContent { get; internal set; }

        public bool IsTextFile { get; internal set; }
    }
}
