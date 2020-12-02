using System;

namespace FileManipulator
{
    public interface IWatcherAction
    {
        string Path { get; set; }
        DateTime Time { get; set; }
    }
}