using System.IO;

namespace FileManipulator.Models.Manipulator.FileInfos
{
    public interface ISourceFileInfo
    {
        string SourceFileName { get; }

        StreamReader SourceFileContent { get; }

        bool IsTextFile { get; }
    }
}