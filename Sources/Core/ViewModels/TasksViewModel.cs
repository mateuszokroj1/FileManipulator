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
    public class TasksViewModel : ViewModelWithModelProperty<ObservableCollection<ITask>>, ITasksViewModel
    {
        #region Constructor

        public TasksViewModel(ObservableCollection<ITask> tasksCollection) : base(tasksCollection)
        {
            CreatePropertyChangedObservable(nameof(Model), () => Model)
                .Subscribe(_ => ConfigureTasksObserver());

            SelectedItemChanged = CreatePropertyChangedObservable(nameof(SelectedItem), () => SelectedItem);
        }

        #endregion

        #region Fields

        private ITask selectedItem;
        private IDisposable disposable;

        #endregion

        #region Properties

        public ObservableCollection<ModelBase> TasksViewModels { get; } = new ObservableCollection<ModelBase>();

        public ITask SelectedItem
        {
            get => this.selectedItem;
            set => SetProperty(ref this.selectedItem, value);
        }

        public IObservable<ITask> SelectedItemChanged { get; }

        #endregion

        #region Methods

        private void ConfigureTasksObserver()
        {
            if(Model != null)
            {
                this.disposable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => Model.CollectionChanged += handler,
                    handler => Model.CollectionChanged -= handler
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
            CheckThatExistsViewModelsForAllModels();

            CheckHaveNotObsoleteViewModels();
        }

        private void CheckHaveNotObsoleteViewModels()
        {
            foreach (var viewModel in TasksViewModels)
            {
                if (viewModel is WatcherViewModel watcherViewModel)
                {
                    if (Model.Where(task => task == watcherViewModel.Watcher).Count() < 1)
                        TasksViewModels.Remove(viewModel);
                }
                else if (viewModel is ManipulatorViewModel manipulatorViewModel)
                {
                    if (Model.Where(task => task == manipulatorViewModel.Model).Count() < 1)
                        TasksViewModels.Remove(viewModel);
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
                        .Where(viewModel => viewModel?.Watcher == watcher)
                        .Count() < 1)
                        TasksViewModels.Add(new WatcherViewModel(watcher));
                }
                else if (task is Manipulator manipulator)
                {
                    if (TasksViewModels
                        .Select(modelBase => modelBase as ManipulatorViewModel)
                        .Where(viewModel => viewModel?.Model == manipulator)
                        .Count() < 1)
                        TasksViewModels.Add(new ManipulatorViewModel(manipulator));
                }
            }
        }

        #endregion
    }
}
