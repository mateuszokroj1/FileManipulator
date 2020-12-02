using System;
using System.ComponentModel;
using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public abstract class Task : ModelBase, ITask
    {
        #region Fields

        private TaskState state = TaskState.Undefined;
        private string name;
        private Exception lastError;

        public TaskEventHandler Starting;
        public TaskEventHandler Started;
        public TaskEventHandler Stopping;
        public TaskEventHandler Stopped;
        public TaskEventHandler Pausing;
        public TaskEventHandler Paused;
        public TaskErrorEventHandler Error;
        public TaskEventHandler Completed;

        #endregion

        #region Properties

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

        public abstract void Dispose();

        #endregion
    }
}
