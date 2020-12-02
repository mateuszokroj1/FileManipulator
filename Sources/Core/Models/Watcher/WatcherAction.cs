using System;

namespace FileManipulator
{
    public abstract class WatcherAction : IWatcherAction
    {
        public string Path { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}