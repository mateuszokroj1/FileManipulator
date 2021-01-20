namespace FileManipulator.Models.Watcher
{
    public interface IChangeWatcherAction
    {
        ChangeType ChangeType { get; set; }
    }
}