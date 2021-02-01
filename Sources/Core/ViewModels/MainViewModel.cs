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

            SelectedItemChanged = CreatePropertyChangedObservable(nameof(SelectedItem), () => SelectedItem);

            SelectedItemChanged.Subscribe(value => TasksViewModel.SelectedItem = value);
            TasksViewModel.SelectedItemChanged.Subscribe(value => SelectedItem = value);

            CreateNewWatcherTaskCommand = new Command(() => CreateNew<Watcher>());
            CreateNewManipulatorTaskCommand = new Command(() => CreateNew<Manipulator>());
            HelpCommand = new Command(() => OpenHelp());
            
            EditTaskNameCommand = new ReactiveCommand(
                SelectedItemChanged.Select(_ => SelectedItem != null),
                () => EditTaskName(RenameDialog));
        }

        #endregion

        #region Fields

        private IViewModelWithModelProperty selectedItem;

        #endregion

        #region Properties

        public ObservableCollection<ITask> Tasks { get; set; } = new ObservableCollection<ITask>();

        public TasksViewModel TasksViewModel { get; }

        public ITextDialog RenameDialog { get; set; }

        public IViewModelWithModelProperty SelectedItem
        {
            get => this.selectedItem;
            set => SetProperty(ref this.selectedItem, value);
        }

        public IObservable<IViewModelWithModelProperty> SelectedItemChanged { get; }

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
        }

        public void EditTaskName(ITextDialog window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            if (SelectedItem == null)
                return;

            ITask task = TasksViewModel.GetSelectedTask();

            window.Value = task.Name;

            if (!window.ShowDialog())
                return;

            task.Name = window.Value;
        }

        public void OpenHelp()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Pomoc.docx");

            if (!File.Exists(path)) return;

            try
            {
                Process.Start(path);
            }
            catch(Win32Exception) // When not setted default app for DOCX
            {
                Process.Start("explorer.exe", $"/select, \"{path}\"");
            }
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
