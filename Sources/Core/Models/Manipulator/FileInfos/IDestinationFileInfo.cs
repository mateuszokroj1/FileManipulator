namespace FileManipulator.Models.Manipulator.FileInfos
{
    public interface IDestinationFileInfo : ISourceFileInfo
    {
        string DestinationFileName { get; set; }

        string DestinationFileContent { get; set; }
    }
}