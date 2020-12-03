using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileManipulator
{
    public class MainViewModel : ModelBase
    {
        #region Constructors

        public MainViewModel() { }

        #endregion

        #region Fields

        private Task selectedTask;

        #endregion

        #region Properties

        public ObservableCollection<Task> Tasks { get; } = new ObservableCollection<Task>();

        public Task SelectedTask
        {
            get => this.selectedTask;
            set => SetProperty(ref this.selectedTask, value);
        }

        public bool IsAnyWorkingTasks =>
            Tasks
            .Where(task => task.State == TaskState.Paused || task.State == TaskState.Working)
            .Count() > 0;

        #region Commands



        #endregion

        #endregion

        #region Methods

        public async void Close(Func<bool> showMessage, Action onCompleted)
        {
            if(IsAnyWorkingTasks)
            {
                if (!showMessage()) return;

                foreach (var task in Tasks)
                    await task.StopAsync();

                Tasks.Clear();
            }

            onCompleted?.Invoke();
        }

        #endregion
    }
}
