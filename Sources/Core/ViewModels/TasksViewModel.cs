using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;
using FileManipulator.ViewModels;

namespace FileManipulator
{
    public class TasksViewModel : ModelBase, ITasksViewModel
    {
        #region Constructor

        public TasksViewModel()
        {
            CreatePropertyChangedObservable(nameof(Tasks), () => Tasks)
                .Subscribe(_ => ConfigureTasksObserver());
        }

        #endregion

        #region Fields

        private ObservableCollection<ITask> tasks;
        private ITask selectedItem;
        private IDisposable disposable;

        #endregion

        #region Properties

        public ObservableCollection<ITask> Tasks
        {
            get => this.tasks;
            set => SetProperty(ref this.tasks, value);
        }

        public ObservableCollection<ModelBase> TasksViewModels { get; } = new ObservableCollection<ModelBase>();

        public ITask SelectedItem
        {
            get => this.selectedItem;
            set => SetProperty(ref this.selectedItem, value);
        }

        #endregion

        #region Methods

        private void ConfigureTasksObserver()
        {
            if(Tasks != null)
            {
                this.disposable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => Tasks.CollectionChanged += handler,
                    handler => Tasks.CollectionChanged -= handler
                )
                .Subscribe(_ => RefreshCollections());
            }
            else
            {
                this.disposable?.Dispose();
                this.disposable = null;
            }
        }

        private void RefreshCollections()
        {
            foreach(var task in Tasks)
            {
                if(task is Watcher watcher)
                {
                    if (TasksViewModels
                        .Select(modelBase => modelBase as WatcherViewModel)
                        .Where(viewModel => viewModel?.Watcher == watcher)
                        .Count() < 1)
                        TasksViewModels.Add(new WatcherViewModel(watcher));
                }
                else if(task is Manipulator manipulator)
                {
                    if (TasksViewModels
                        .Select(modelBase => modelBase as ManipulatorViewModel)
                        .Where(viewModel => viewModel?.Manipulator == manipulator)
                        .Count() < 1)
                        TasksViewModels.Add(new ManipulatorViewModel(manipulator));
                }
            }

            foreach(var viewModel in TasksViewModels)
            {
                if(viewModel is WatcherViewModel watcherViewModel)
                {
                    if (Tasks.Where(task => task == watcherViewModel.Watcher).Count() < 1)
                        TasksViewModels.Remove(viewModel);
                }
                else if(viewModel is ManipulatorViewModel manipulatorViewModel)
                {
                    if (Tasks.Where(task => task == manipulatorViewModel.Manipulator).Count() < 1)
                        TasksViewModels.Remove(viewModel);
                }
            }
        }

        #endregion
    }
}
