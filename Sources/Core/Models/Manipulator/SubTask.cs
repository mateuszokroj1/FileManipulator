using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FileManipulator.Models.Manipulator
{
    public abstract class SubTask : ModelBase, ISubTask
    {
        protected SubTask()
        {
            StateChanged = CreatePropertyChangedObservable(nameof(State), () => State);
        }

        private SubTaskState state;

        public SubTaskState State
        {
            get => this.state;
            set => SetProperty(ref this.state, value);
        }

        public IObservable<SubTaskState> StateChanged { get; }

        public SynchronizationContext SynchronizationContext { get; } = SynchronizationContext.Current;


    }
}
