using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManipulator.UI
   
{
    public class MainViewModel : ModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            
        }

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

        #endregion

        #region Methods

        public async void Close(Func<bool> canClose, CancelEventArgs e)
        {
            if (IsAnyWorkingTasks)
            {
                if (!canClose())
                {
                    e.Cancel = true;
                    return;
                }

                foreach (var task in Tasks)
                    await task.StopAsync();

                Tasks.Clear();
            }
        }

        #endregion

    }
}
