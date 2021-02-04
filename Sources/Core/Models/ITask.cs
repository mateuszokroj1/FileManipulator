using System;
using System.ComponentModel;
using System.Windows.Input;

using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public interface ITask : IDisposable, INotifyPropertyChanged
    {
        #region Properties

        TaskState State { get; }

        TaskProgress Progress { get; }

        string Name { get; set; }

        Guid Id { get; }

        Exception LastError { get; set; }
        ICommand CloseCommand { get; }

        #endregion

        #region Methods

        STT.Task StartAsync();

        STT.Task StopAsync();

        STT.Task PauseAsync();

        STT.Task ResetAsync();

        string GenerateJson();

        bool LoadJson(string content);

        #endregion
    }
}
