namespace FileManipulator
{
    public interface IChangeWatcherAction
    {
        ChangeType ChangeType { get; set; }
    }
}