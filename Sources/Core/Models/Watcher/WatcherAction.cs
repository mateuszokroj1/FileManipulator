using System;

using FileManipulator.Helpers;

namespace FileManipulator.Models.Watcher
{
    public abstract class WatcherAction : IWatcherAction
    {
        public string Path { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;

        public string ActionType => WatcherActionTypeConverter.GetString(GetType());
    }
}