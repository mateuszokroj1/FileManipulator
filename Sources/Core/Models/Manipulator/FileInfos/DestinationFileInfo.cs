namespace FileManipulator.Models.Manipulator.FileInfos
{
    public class DestinationFileInfo : SourceFileInfo, ISourceFileInfo, IDestinationFileInfo
    {
        public DestinationFileInfo() { }

        public DestinationFileInfo(SourceFileInfo fileInfo)
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
