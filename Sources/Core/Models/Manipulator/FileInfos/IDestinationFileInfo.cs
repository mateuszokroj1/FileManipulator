namespace FileManipulator.Models.Manipulator.FileInfos
{
    public interface IDestinationFileInfo
    {
        string DestinationFileName { get; set; }

        string DestinationFileContent { get; set; }
    }
}