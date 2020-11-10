using System;
using System.ComponentModel;

namespace FileManipulator
{
    public interface ITask : IDisposable, INotifyPropertyChanged
    {
        #region Properties

        TaskState State { get; }

        IProgress<float> Progress { get; }

        string Name { get; set; }

        Guid Id { get; }

        #endregion

        #region Methods

        Task StartAsync();

        Task StopAsync();

        Task PauseAsync();

        #endregion
    }
}
