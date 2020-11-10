using System;

namespace FileManipulator
{
    public interface ITaskErrorEventArgs
    {
        Exception Exception { get; }
    }
}