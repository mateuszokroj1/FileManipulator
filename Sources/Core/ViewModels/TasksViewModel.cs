using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;
using FileManipulator.ViewModels;

namespace FileManipulator
{
    public class TasksViewModel : ViewModelWithModelProperty<ObservableCollection<ITask>>, ITasksViewModel
    {
        #region Constructor

        public TasksViewModel(ObservableCollection<ITask> tasksCollection) : base(tasksCollection)
        {
            ConfigureTasksObserver();

            SelectedItemChanged = CreatePropertyChangedObservable(nameof(SelectedItem), () => SelectedItem);
        }

        #endregion

        #region Fields

        private IViewModelWithModelProperty selectedItem;
        private IDisposable disposable;

        #endregion

        #region Properties

        public ObservableCollection<IViewModelWithModelProperty> TasksViewModels { get; } = new ObservableCollection<IViewModelWithModelProperty>();

        public IViewModelWithModelProperty SelectedItem
        {
            get => this.selectedItem;
            set => SetProperty(ref this.selectedItem, value);
        }

        public IObservable<IViewModelWithModelProperty> SelectedItemChanged { get; }

        #endregion

        #region Methods

        public void SelectTask(ITask task)
        {
            IViewModelWithModelProperty viewModel = null;

            if(task is Watcher watcher)
            {
                foreach(var vm in TasksViewModels)
                {
                    if(vm is WatcherViewModel wvw && wvw.Model == watcher)
                    {
                        viewModel = vm;
                        break;
                    }
                }
            }
            else if(task is Manipulator manipulator)
            {
                foreach (var vm in TasksViewModels)
                {
                    if (vm is ManipulatorViewModel mvw && mvw.Model == manipulator)
                    {
                        viewModel = vm;
                        break;
                    }
                }
            }

            SelectedItem = viewModel;
        }

        public ITask GetSelectedTask()
        {
            if (SelectedItem is WatcherViewModel watcher)
                return watcher.Model;
            else if (SelectedItem is ManipulatorViewModel manipulator)
                return manipulator.Model;
            else
                return null;
        }

        private void ConfigureTasksObserver()
        {
            this.disposable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => Model.CollectionChanged += handler,
                handler => Model.CollectionChanged -= handler
            )
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(_ => RefreshCollections());
        }

        private void RefreshCollections()
        {
            CheckHaveNotObsoleteViewModels();
            CheckThatExistsViewModelsForAllModels();
        }

        private void CheckHaveNotObsoleteViewModels()
        {
            while (true)
            {
                if (TasksViewModels.Count < 1)
                    return;

                foreach (var viewModel in TasksViewModels)
                {
                    if (viewModel is WatcherViewModel watcherViewModel)
                    {
                        if (Model.Where(task => task == watcherViewModel.Model).Count() < 1)
                        {
                            TasksViewModels.Remove(viewModel);
                            break;
                        }
                    }
                    else if (viewModel is ManipulatorViewModel manipulatorViewModel)
                    {
                        if (Model.Where(task => task == manipulatorViewModel.Model).Count() < 1)
                        {
                            TasksViewModels.Remove(viewModel);
                            break;
                        }
                    }

                    if (TasksViewModels.LastOrDefault() == viewModel)
                        return;
                }
            }
        }

        private void CheckThatExistsViewModelsForAllModels()
        {
            foreach (var task in Model)
            {
                if (task is Watcher watcher)
                {
                    if (TasksViewModels
                        .Select(modelBase => modelBase as WatcherViewModel)
                        .Where(viewModel => viewModel?.Model == watcher)
                        .Count() < 1)
                    {
                        var newItem = new WatcherViewModel(watcher);
                        TasksViewModels.Add(newItem);
                        SelectedItem = newItem;
                    }
                        
                }
                else if (task is Manipulator manipulator)
                {
                    if (TasksViewModels
                        .Select(modelBase => modelBase as ManipulatorViewModel)
                        .Where(viewModel => viewModel?.Model == manipulator)
                        .Count() < 1)
                    {
                        var newItem = new ManipulatorViewModel(manipulator);
                        TasksViewModels.Add(newItem);
                        SelectedItem = newItem;
                    }
                        
                }
            }
        }

        #endregion
    }
}
