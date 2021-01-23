using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;

using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public abstract class Task : ModelBase, ITask
    {
        #region Fields

        private TaskState state = TaskState.Undefined;
        private string name;
        private Exception lastError;

        #endregion

        #region Properties

        public Subject<TaskEventArgs> Starting { get; } = new Subject<TaskEventArgs>();

        public Subject<TaskEventArgs> Started { get; } = new Subject<TaskEventArgs>();

        public Subject<TaskEventArgs> Stopping { get; } = new Subject<TaskEventArgs>();

        public Subject<TaskEventArgs> Stopped { get; } = new Subject<TaskEventArgs>();

        public Subject<TaskEventArgs> Pausing { get; } = new Subject<TaskEventArgs>();

        public Subject<TaskEventArgs> Paused { get; } = new Subject<TaskEventArgs>();

        public Subject<TaskErrorEventArgs> Error { get; } = new Subject<TaskErrorEventArgs>();

        public Subject<TaskEventArgs> Completed { get; } = new Subject<TaskEventArgs>();

        /// <summary>
        /// Actual task state
        /// </summary>
        [DefaultValue(TaskState.Undefined)]
        public virtual TaskState State
        {
            get => this.state;
            protected set => SetProperty(ref this.state, value);
        }

        /// <summary>
        /// Provides progress reporting
        /// </summary>
        public TaskProgress Progress { get; } = new TaskProgress();

        /// <summary>
        /// Unique id
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Last handled <see cref="Exception"/>, when State is Error.
        /// </summary>
        public virtual Exception LastError
        {
            get => this.lastError;
            set => SetProperty(ref this.lastError, value);
        }

        public virtual string Name
        {
            get => this.name;
            set => SetProperty(ref this.name, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts task
        /// </summary>
        public abstract STT.Task StartAsync();

        /// <summary>
        /// Stops task
        /// </summary>
        public abstract STT.Task StopAsync();

        /// <summary>
        /// Pauses task
        /// </summary>
        public abstract STT.Task PauseAsync();

        /// <summary>
        /// Resets state of task
        /// </summary>
        public abstract STT.Task ResetAsync();

        public virtual void Close(ICollection<Task> collection)
        {
            collection.Remove(this);
        }

        public abstract void Dispose();

        #endregion
    }
}
