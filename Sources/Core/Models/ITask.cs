using System;
using System.ComponentModel;

using STT = System.Threading.Tasks;

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

        STT.Task StartAsync();

        STT.Task StopAsync();

        STT.Task PauseAsync();

        STT.Task ResetAsync();

        #endregion
    }
}
