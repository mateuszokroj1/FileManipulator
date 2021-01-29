using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Manipulations;

namespace FileManipulator.ViewModels
{
    public class ManipulatorViewModel : ViewModelWithModelProperty<Manipulator>
    {
        #region Constructor

        public ManipulatorViewModel(Manipulator manipulator) : base(manipulator)
        {

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

        public bool CanStart { get; }//TODO
        public bool CanStop { get; }
        public bool CanEdit => Model.State == TaskState.Ready;

        public IObservable<bool> CanStartChanged { get; }
        public IObservable<bool> CanStopChanged { get; }
        public IObservable<bool> CanEditChanged { get; }

        #endregion

        #region Methods

        #endregion
    }
}
