using System;

namespace FileManipulator
{
    public class TaskErrorEventArgs : TaskEventArgs, ITaskErrorEventArgs
    {
        #region Constructor

        public TaskErrorEventArgs(Exception exception, Task task) : base(task)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        #endregion

        #region Properties

        public Exception Exception { get; }

        #endregion
    }
}
