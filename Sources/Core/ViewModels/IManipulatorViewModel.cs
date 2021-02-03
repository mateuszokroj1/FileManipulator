using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Manipulations;

namespace FileManipulator.ViewModels
{
    public interface IManipulatorViewModel : IDisposable
    {
        ICommand BrowseCommand { get; }

        bool CanEdit { get; }

        IObservable<bool> CanEditChanged { get; }

        bool CanStart { get; }

        IObservable<bool> CanStartChanged { get; }

        bool CanStop { get; }

        IObservable<bool> CanStopChanged { get; }

        IEnumerable<IContentFilter> ContentFilters { get; }

        IEnumerable<IContentManipulation> ContentManipulations { get; }

        FilesSelectorViewModel FilesSelectorViewModel { get; }

        bool IsMoving { get; set; }

        IEnumerable<INameFilter> NameFilters { get; }

        IEnumerable<INameManipulation> NameManipulations { get; }

        string OutputDirectory { get; set; }

        TaskProgress Progress { get; }

        ICommand StartCommand { get; }

        ICommand StopCommand { get; }

        void Browse();
        void Start();
        void Stop();
    }
}