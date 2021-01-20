namespace FileManipulator.Models.Watcher
{
    public class ChangeWatcherAction : WatcherAction, IChangeWatcherAction
    {
        public ChangeType ChangeType { get; set; }

        public override string ToString()
        {
            switch (ChangeType)
            {
                case ChangeType.Created:
                    return string.Format(Messages.Watcher_CreatedMessage, Path);
                case ChangeType.Deleted:
                    return string.Format(Messages.Watcher_DeletedMessage, Path);
                case ChangeType.Modified:
                    return string.Format(Messages.Watcher_ModifiedMessage, Path);
                default:
                    return string.Empty;
            }
        }
    }
}
