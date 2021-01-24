namespace FileManipulator.Models.Manipulator.FileInfos
{
    public class DestinationFileInfo : ISourceFileInfo, ISourceFileInfo, IDestinationFileInfo
    {
        public DestinationFileInfo() { }

        public DestinationFileInfo(ISourceFileInfo fileInfo)
        {
            SourceFileName = fileInfo.SourceFileName;
            SourceFileContent = fileInfo.SourceFileContent;
            IsTextFile = fileInfo.IsTextFile;

            DestinationFileName = SourceFileName;

            if (IsTextFile)
                DestinationFileContent = SourceFileContent.ReadToEnd();
        }

        public string DestinationFileName { get; set; }

        public string DestinationFileContent { get; set; }
    }
}
