namespace FileManipulator.Models.Watcher
{
    public interface IRenameWatcherAction
    {
        string DestinationPath { get; set; }
    }
}