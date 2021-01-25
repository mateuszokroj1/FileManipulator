using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;

using FileManipulator.Models.Watcher;

namespace FileManipulator.ViewModels
{
    public class WatcherViewModel : ModelBase
    {
        #region Constructors

        public WatcherViewModel(Watcher model)
        {
            Watcher = model ?? throw new ArgumentNullException(nameof(model));

            this.propertyChangedObservable =
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>
                (
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler
                )
                .Where(args => !string.IsNullOrWhiteSpace(args?.EventArgs?.PropertyName))
                .Select(args => args.EventArgs.PropertyName);

            /*ClearCommand = new Command(() => Clear());
            BrowseCommand = new Command();

            StartCommand = new ReactiveCommand();
            StopCommand = new ReactiveCommand();*/

            IsDirectoryPath = true;
        }

        #endregion

        #region Fields

        private readonly IObservable<string> propertyChangedObservable;
        private bool isDirectoryPath;
        private string path;
        private bool includeSubdirectories;

        #endregion

        #region Properties

        public Watcher Watcher { get; }

        public string Path
        {
            get => this.path;
            set => SetProperty(ref this.path, value);
        }

        public bool IncludeSubdirectories
        {
            get => this.includeSubdirectories;
            set => SetProperty(ref this.includeSubdirectories, value);
        }

        public ObservableCollection<WatcherAction> Actions => Watcher.Actions;

        public bool IsDirectoryPath
        {
            get => this.isDirectoryPath;
            set => SetProperty(ref this.isDirectoryPath, value);
        }

        public ICommand StartCommand { get; }

        public ICommand StopCommand { get; }

        public ICommand ClearCommand { get; }

        public ICommand BrowseCommand { get; }

        #endregion

        #region Methods

        public void Clear() => Actions.Clear();

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Browse()
        {

        }

        #endregion
    }
}
