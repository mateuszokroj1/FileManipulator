using System;

namespace FileManipulator.Models.Watcher
{
    public interface IWatcherAction
    {
        string Path { get; set; }
        DateTime Time { get; set; }
    }
}