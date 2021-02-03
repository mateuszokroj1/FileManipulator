namespace FileManipulator.Models.Manipulator.FileInfos
{
    public class DestinationFileInfo : SourceFileInfo, IDestinationFileInfo
    {
        public DestinationFileInfo() { }

        public DestinationFileInfo(ISourceFileInfo fileInfo)
        {
            SourceFileName = fileInfo.SourceFileName;
            SourceFileContent = null;
            IsTextFile = fileInfo.IsTextFile;

            DestinationFileName = SourceFileName;

            if (IsTextFile && fileInfo.SourceFileContent?.BaseStream?.Length > 0)
            {
                fileInfo.SourceFileContent.BaseStream.Position = 0;
                DestinationFileContent = fileInfo.SourceFileContent.ReadToEnd();
            }

            fileInfo.SourceFileContent.BaseStream.Close();
            fileInfo.SourceFileContent.Dispose();
        }

        public string DestinationFileName { get; set; }

        public string DestinationFileContent { get; set; }
    }
}
