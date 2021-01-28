using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;

namespace FileManipulator.ViewModels

{
    public class MainViewModel : ModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            TasksViewModel = new TasksViewModel(Tasks);

            SelectedTaskChanged = CreatePropertyChangedObservable(nameof(SelectedTask), () => SelectedTask);

            SelectedTaskChanged.Subscribe(value => TasksViewModel.SelectedItem = value);
            TasksViewModel.SelectedItemChanged.Subscribe(value => SelectedTask = value);

            CreateNewWatcherTaskCommand = new Command(() => CreateNew<Watcher>());
            CreateNewManipulatorTaskCommand = new Command(() => CreateNew<Manipulator>());
            HelpCommand = new Command(() => Process.Start(Path.Combine(Environment.CurrentDirectory, "Pomoc.docx")));
            
            EditTaskNameCommand = new ReactiveCommand(
                SelectedTaskChanged.Select(_ => SelectedTask != null),
                () => EditTaskName(RenameDialog));
        }

        #endregion

        #region Fields

        private ITask selectedTask;

        #endregion

        #region Properties

        public ObservableCollection<ITask> Tasks { get; set; } = new ObservableCollection<ITask>();

        public TasksViewModel TasksViewModel { get; }

        public ITextDialog RenameDialog { get; set; }

        public ITask SelectedTask
        {
            get => this.selectedTask;
            set => SetProperty(ref this.selectedTask, value);
        }

        public IObservable<ITask> SelectedTaskChanged { get; }

        public bool IsAnyWorkingTasks =>
            Tasks
            .Where(task => task.State == TaskState.Paused || task.State == TaskState.Working)
            .Count() > 0;

        public ICommand CreateNewWatcherTaskCommand { get; }

        public ICommand CreateNewManipulatorTaskCommand { get; }

        public ICommand EditTaskNameCommand { get; }

        public ICommand HelpCommand { get; }

        public Func<bool> MessageOnCloseWhileTaskWorking { get; set; }

        #endregion

        #region Methods

        public void CreateNew<TaskType>()
            where TaskType : Task
        {
            if (Tasks == null || Tasks.Count > 100)
                return;

            ITask newTask;
            if (typeof(TaskType) == typeof(Watcher))
            {
                newTask = new Watcher(Tasks);
            }
            else if (typeof(TaskType) == typeof(Manipulator))
            {
                newTask = new Manipulator(Tasks, MessageOnCloseWhileTaskWorking);
            }
            else
                throw new InvalidOperationException("Invalid task type.");

            Tasks.Add(newTask);
            SelectedTask = newTask;
        }

        public void EditTaskName(ITextDialog window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            if (SelectedTask == null)
                return;

            window.Value = SelectedTask.Name;

            if (!window.ShowDialog())
                return;

            SelectedTask.Name = window.Value;
        }

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
