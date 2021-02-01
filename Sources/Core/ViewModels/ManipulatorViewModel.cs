using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Manipulations;

namespace FileManipulator.ViewModels
{
    public class ManipulatorViewModel : ViewModelWithModelProperty<Manipulator>, IManipulatorViewModel
    {
        #region Constructor

        public ManipulatorViewModel(Manipulator manipulator) : base(manipulator)
        {
            StateChanged = Model.CreatePropertyChangedObservable(nameof(Model.State), () => Model.State);
            CanStartChanged = CreatePropertyChangedObservable(nameof(CanStart), () => CanStart);
            CanStopChanged = CreatePropertyChangedObservable(nameof(CanStop), () => CanStop);
            CanEditChanged = CreatePropertyChangedObservable(nameof(CanEdit), () => CanEdit);

            StateChanged.Subscribe(_ =>
            {
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
                OnPropertyChanged(nameof(CanEdit));
            });

            CreatePropertyChangedObservable(nameof(OutputDirectory), () => OutputDirectory)
                .Subscribe(_ => OnPropertyChanged(nameof(CanStart)));

            CreatePropertyChangedObservable(nameof(CanStart), () => CanStart)
                .Merge(CreatePropertyChangedObservable(nameof(CanStop), () => CanStop))
                .Subscribe(_ => OnPropertyChanged(nameof(CanEdit)));

            StartCommand = new ReactiveCommand(CanStartChanged, () => Start());
            StopCommand = new ReactiveCommand(CanStopChanged, () => Stop());
        }

        #endregion

        #region Fields

        private bool isMoving;
        private string outputDirectory;

        #endregion

        #region Properties

        public ObservableCollection<string> InputPaths { get; } = new ObservableCollection<string>();

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

        public FilesSelectorViewModel FilesSelectorViewModel { get; } = new FilesSelectorViewModel();

        public bool CanStart => Model.State == TaskState.Ready &&
            InputPaths.Count > 0 &&
            ((!IsMoving) || (IsMoving && !string.IsNullOrWhiteSpace(OutputDirectory)));
        public bool CanStop => Model.State == TaskState.Working || Model.State == TaskState.Paused;
        public bool CanEdit => Model.State == TaskState.Ready;

        public TaskProgress Progress => Model.Progress;

        public IObservable<bool> CanStartChanged { get; }
        public IObservable<bool> CanStopChanged { get; }
        public IObservable<bool> CanEditChanged { get; }
        public IObservable<TaskState> StateChanged { get; }

        public Func<string,string> GetDirectoryFromDialog { get; set; }

        #endregion

        #region Methods

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Browse()
        {
            OutputDirectory = GetDirectoryFromDialog(OutputDirectory);
        }

        #endregion
    }
}
