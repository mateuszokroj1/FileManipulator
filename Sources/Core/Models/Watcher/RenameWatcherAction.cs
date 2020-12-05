namespace FileManipulator
{
    public class RenameWatcherAction : WatcherAction, IRenameWatcherAction
    {
        public string DestinationPath { get; set; }

        public override string ToString() =>
            string.Format(Messages.Watcher_RenamedMessage, Path, DestinationPath);
    }
}
