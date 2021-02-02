using System;
using System.Threading;
using System.Windows.Input;

namespace FileManipulator.Models.Manipulator
{
    public abstract class SubTask : ModelBase, ISubTask
    {
        protected SubTask()
        {
            StateChanged = CreatePropertyChangedObservable(nameof(State), () => State);

            CloseCommand = new Command(() => Close());
        }

        private SubTaskState state;

        public SubTaskState State
        {
            get => this.state;
            set => SetProperty(ref this.state, value);
        }

        public IObservable<SubTaskState> StateChanged { get; }

        public ICommand CloseCommand { get; }

        public SynchronizationContext SynchronizationContext { get; } = SynchronizationContext.Current;

        public abstract void Close();
    }
}
