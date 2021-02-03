using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Filters.NameFilters;
using FileManipulator.Models.Manipulator.Manipulations;
using FileManipulator.Models.Manipulator.Manipulations.NameManipulations;

namespace FileManipulator.ViewModels
{
    public class ManipulatorViewModel : ViewModelWithModelProperty<Manipulator>, IManipulatorViewModel
    {
        #region Constructor

        public ManipulatorViewModel(Manipulator manipulator) : base(manipulator)
        {
            CanStartChanged = CreatePropertyChangedObservable(nameof(CanStart), () => CanStart);
            CanStopChanged = CreatePropertyChangedObservable(nameof(CanStop), () => CanStop);
            CanEditChanged = CreatePropertyChangedObservable(nameof(CanEdit), () => CanEdit);
            OutputDirectoryChanged = CreatePropertyChangedObservable(nameof(OutputDirectory), () => OutputDirectory);
            IsMovingChanged = CreatePropertyChangedObservable(nameof(IsMoving), () => IsMoving);

            this.disposables[0] = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => FilesSelectorViewModel.Files.CollectionChanged += handler,
                handler => FilesSelectorViewModel.Files.CollectionChanged -= handler
            )
            .Subscribe(_ =>
            {
                Model.FilePaths = FilesSelectorViewModel.Files;
                OnPropertyChanged(nameof(CanStart));
            });

            this.disposables[1] = Model.StateChanged.Subscribe(_ =>
            {
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
                OnPropertyChanged(nameof(CanEdit));
            });

            this.disposables[2] = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => Model.Filters.CollectionChanged += handler,
                handler => Model.Filters.CollectionChanged -= handler
            )
            .Subscribe(_ =>
            {
                NameFilters = Model.Filters
                    .Select(filter => filter as INameFilter)
                    .Where(filter => filter != null);

                ContentFilters = Model.Filters
                    .Select(filter => filter as IContentFilter)
                    .Where(filter => filter != null);

                OnPropertyChanged(nameof(NameFilters));
                OnPropertyChanged(nameof(ContentFilters));
            });

            this.disposables[3] = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => Model.Manipulations.CollectionChanged += handler,
                handler => Model.Manipulations.CollectionChanged -= handler
            )
            .Subscribe(_ =>
            {
                NameManipulations = Model.Manipulations
                    .Select(filter => filter as INameManipulation)
                    .Where(filter => filter != null);

                ContentManipulations = Model.Manipulations
                    .Select(filter => filter as IContentManipulation)
                    .Where(filter => filter != null);

                OnPropertyChanged(nameof(NameManipulations));
                OnPropertyChanged(nameof(ContentManipulations));
            });

            this.disposables[4] = OutputDirectoryChanged.Subscribe(_ =>
            {
                Model.DestinationDir = IsMoving ? OutputDirectory : null;
            });
            this.disposables[5] = IsMovingChanged.Subscribe(_ =>
            {
                Model.DestinationDir = IsMoving ? OutputDirectory : null;
            });

            StartCommand = new ReactiveCommand(CanStartChanged, () => Start());
            StopCommand = new ReactiveCommand(CanStopChanged, () => Stop());
            BrowseCommand = new Command(() => Browse());

            AddFilterCommand = new Command(type => AddFilter(type as Type));
            AddManipulationCommand = new Command(type => AddManipulation(type as Type));
        }

        #endregion

        #region Fields

        private bool isMoving;
        private string outputDirectory;
        private readonly IDisposable[] disposables = new IDisposable[6];

        #endregion

        #region Properties

        public bool IsMoving
        {
            get => this.isMoving;
            set => SetProperty(ref this.isMoving, value);
        }

        public string OutputDirectory
        {
            get => this.outputDirectory;
            set => SetProperty(ref this.outputDirectory, value);
        }

        public IEnumerable<INameFilter> NameFilters { get; private set; }
        public IEnumerable<IContentFilter> ContentFilters { get; private set; }
        public IEnumerable<INameManipulation> NameManipulations { get; private set; }
        public IEnumerable<IContentManipulation> ContentManipulations { get; private set; }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand BrowseCommand { get; }
        public ICommand AddFilterCommand { get; }
        public ICommand AddManipulationCommand { get; }

        public FilesSelectorViewModel FilesSelectorViewModel { get; } = new FilesSelectorViewModel();

        public bool CanStart => Model.State == TaskState.Ready &&
            FilesSelectorViewModel.Files.Count > 0 &&
            ((!IsMoving) || (IsMoving && !string.IsNullOrWhiteSpace(OutputDirectory)));
        public bool CanStop => Model.State == TaskState.Working || Model.State == TaskState.Paused;
        public bool CanEdit => Model.State == TaskState.Ready;

        public TaskProgress Progress => Model.Progress;

        public IObservable<bool> CanStartChanged { get; }
        public IObservable<bool> CanStopChanged { get; }
        public IObservable<bool> CanEditChanged { get; }
        public IObservable<TaskState> StateChanged { get; }
        public IObservable<string> OutputDirectoryChanged { get; }
        public IObservable<bool> IsMovingChanged { get; }

        public Func<string,string> GetDirectoryFromDialog { get; set; }

        #endregion

        #region Methods

        public void Start()
        {
            if (!MainViewModel.ConfirmationOnStart())
                return;

            Model.StartAsync();
        }

        public void Stop()
        {
            Model.StopAsync();
        }

        public void Browse()
        {
            OutputDirectory = GetDirectoryFromDialog(OutputDirectory);
        }

        public void AddFilter(Type type)
        {
            if (typeof(ClassicSorting) == type)
                Model.Filters.Add(new ClassicSorting(Model.Filters));
            else if (typeof(AlphanumericSorting) == type)
                Model.Filters.Add(new AlphanumericSorting(Model.Filters));
            else if (typeof(RegexSearcher) == type)
                Model.Filters.Add(new RegexSearcher(Model.Filters));
            else if (typeof(Models.Manipulator.Filters.ContentFilters.RegexSearcher) == type)
                Model.Filters.Add(new Models.Manipulator.Filters.ContentFilters.RegexSearcher(Model.Filters));
        }

        public void AddManipulation(Type type)
        {
            if (typeof(Models.Manipulator.Manipulations.ContentManipulations.Replace) == type)
                Model.Manipulations.Add(new Models.Manipulator.Manipulations.ContentManipulations.Replace(Model.Manipulations));
            else if (typeof(Replace) == type)
                Model.Manipulations.Add(new Replace(Model.Manipulations));
            else if (typeof(SequentialNaming) == type)
                Model.Manipulations.Add(new SequentialNaming(Model.Manipulations));
        }

        public void Dispose()
        {
            foreach (var disposable in this.disposables)
                disposable?.Dispose();
        }

        #endregion
    }
}
