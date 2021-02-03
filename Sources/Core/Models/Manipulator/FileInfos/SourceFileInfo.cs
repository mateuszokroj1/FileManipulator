using System.IO;

namespace FileManipulator.Models.Manipulator.FileInfos
{
    public class SourceFileInfo : ISourceFileInfo
    {
        public string SourceFileName { get; internal set; }

        public StreamReader SourceFileContent { get; set; }

        public bool IsTextFile { get; internal set; }
    }
}
