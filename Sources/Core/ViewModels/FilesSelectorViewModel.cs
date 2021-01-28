using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileManipulator.ViewModels
{
    public class FilesSelectorViewModel : ModelBase
    {
        #region Constructor

        public FilesSelectorViewModel()
        {
            this.propertyChangedObservable = 
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>
                (
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler
                )
                .Where(args => !string.IsNullOrWhiteSpace(args?.EventArgs?.PropertyName))
                .Select(args => args.EventArgs.PropertyName);

            BrowseCommand = new Command(() => Browse());
            DeleteCommand = new ReactiveCommand(
               this.propertyChangedObservable
               .Where(name => name == nameof(SelectedFile))
               .Select(n => Files.Contains(SelectedFile)),
               () => DeleteSelectedFile()
            );
            ClearCommand = new Command(() => ClearList());
        }

        #endregion

        #region Fields

        private SelectionType selectionType = SelectionType.Directory;
        private string directory;
        private bool isDirectoryValid;
        private bool isFilesValid;
        private string selectedFile;
        private IObservable<string> propertyChangedObservable;

        #endregion

        #region Properties

        public SelectionType SelectionType
        {
            get => this.selectionType;
            set => SetProperty(ref this.selectionType, value, nameof(SelectionType), nameof(SelectedFiles));
        }

        public ObservableCollection<string> Files { get; } = new ObservableCollection<string>();

        public string SelectedFile
        {
            get => this.selectedFile;
            set => SetProperty(ref this.selectedFile, value);
        }

        public string Directory
        {
            get => this.directory;
            set => SetProperty(ref this.directory, value, nameof(Directory), nameof(SelectedFiles));
        }

        public bool IsDirectoryValid
        {
            get => this.isDirectoryValid;
            private set => SetProperty(ref this.isDirectoryValid, value);
        }

        public bool IsFilesValid
        {
            get => this.isFilesValid;
            private set => SetProperty(ref this.isFilesValid, value);
        }

        public IEnumerable<string> SelectedFiles => Generate();

        public ICommand BrowseCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand ClearCommand { get; }

        #endregion

        #region Methods

        public IEnumerable<string> Generate()
        {
            switch(SelectionType)
            {
                case SelectionType.Directory:
                    if(!string.IsNullOrEmpty(Directory) && System.IO.Directory.Exists(Directory))
                    {
                        IEnumerable<string> files;

                        try
                        {
                            files = System.IO.Directory.EnumerateFiles(Directory, "", SearchOption.TopDirectoryOnly);
                            IsDirectoryValid = true;
                            return files;
                        }
                        catch(Exception)
                        {
                            IsDirectoryValid = false;
                            return Enumerable.Empty<string>();
                        }
                    }
                    else
                    {
                        IsDirectoryValid = false;
                        return Enumerable.Empty<string>();
                    }
                case SelectionType.Files:
                    foreach (var path in Files)
                    {
                        try
                        {
                            if (!File.Exists(path))
                            {
                                IsFilesValid = false;
                                return Enumerable.Empty<string>();
                            }
                        }
                        catch (Exception)
                        {
                            IsFilesValid = false;
                            return Enumerable.Empty<string>();
                        }
                    }

                    IsFilesValid = true;
                    return Files;
                default:
                    return Enumerable.Empty<string>();
            }
        }

        public void Browse()
        {
            if(SelectionType == SelectionType.Directory)
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.AllowNonFileSystemItems = false;
                    dialog.Title = "Wybierz katalog";
                    dialog.IsFolderPicker = true;
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    dialog.EnsurePathExists = true;
                    dialog.Multiselect = false;
                    dialog.NavigateToShortcut = true;
                    dialog.ShowHiddenItems = false;

                    if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                    Directory = dialog.FileName;
                }
            }
            else if(SelectionType == SelectionType.Files)
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.AllowNonFileSystemItems = false;
                    dialog.Title = "Wybierz pliki do dodania";
                    dialog.IsFolderPicker = false;
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    dialog.EnsureFileExists = true;
                    dialog.Multiselect = true;
                    dialog.NavigateToShortcut = true;
                    dialog.ShowHiddenItems = false;
                    dialog.EnsureReadOnly = false;

                    if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                    if (dialog.FileNames.Count() < 1) return;

                    try
                    {
                        if (dialog.FileNames.Where(file => !File.Exists(file)).Count() > 0) return;
                    }
                    catch(Exception)
                    {
                        return;
                    }

                    foreach (var file in dialog.FileNames)
                        Files.Add(file);
                }
            }
        }

        public void DeleteSelectedFile()
        {
            if (Files.Contains(SelectedFile))
                Files.Remove(SelectedFile);
        }

        public void ClearList() => Files.Clear();

        #endregion
    }
}
