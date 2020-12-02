using System;
using System.Collections.ObjectModel;

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

        #region Commands



        #endregion

        #endregion

        #region Methods

        public void Close(Action onCompleted)
        {

            onCompleted?.Invoke();
        }

        #endregion
    }
}
