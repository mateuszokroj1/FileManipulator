using System;

namespace FileManipulator
{
    public class TaskEventArgs : EventArgs, ITaskEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates new instance of <see cref="TaskEventArgs"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public TaskEventArgs(Task task)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));
        }

        #endregion

        #region Properties

        public Task Task { get; }

        #endregion
    }
}
