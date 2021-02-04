using System;
using System.ComponentModel;
using System.Threading;

namespace FileManipulator.Models.Manipulator
{
    public interface ISubTask : INotifyPropertyChanged
    {
        SubTaskState State { get; set; }

        IObservable<SubTaskState> StateChanged { get; }

        SynchronizationContext SynchronizationContext { get; }

        object GetSimpleObject();

        bool LoadFromSimpleObject(dynamic simpleObject);

        void Close();
    }
}
