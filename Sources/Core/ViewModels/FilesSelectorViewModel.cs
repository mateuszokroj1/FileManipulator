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
            BrowseCommand = new Command(() => Browse());
            DeleteCommand = new ReactiveCommand(
               PropertyChangedObservable
               .Where(name => name == nameof(SelectedFile))
               .Select(n => Files.Contains(SelectedFile)),
               () => DeleteSelectedFile()
            );
            ClearCommand = new Command(() => ClearList());

            PropertyChangedObservable.Where(name => name == nameof(Directory) || name == nameof(SelectionType))
                .Subscribe(_ => RefreshList());
        }

        #endregion

        #region Fields

        private SelectionType selectionType = SelectionType.Directory;
        private string directory;
        private bool isDirectoryValid;
        private bool isFilesValid;
        private string selectedFile;

        #endregion

        #region Properties

        public SelectionType SelectionType
        {
            get => this.selectionType;
            set => SetProperty(ref this.selectionType, value);
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
            set => SetProperty(ref this.directory, value, nameof(Directory));
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

        public ICommand BrowseCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand ClearCommand { get; }

        public Func<bool> RequiredAdministratorForThisFiles { get; set; }

        #endregion

        #region Methods

        private void RefreshList()
        {
            Files.Clear();

            if (SelectionType != SelectionType.Directory)
                return;

            if (!string.IsNullOrEmpty(Directory) && System.IO.Directory.Exists(Directory))
            {
                if(CheckIsRequiredAdministratorPrivileges(Directory))
                {
                    if (!RequiredAdministratorForThisFiles())
                    {
                        Directory = null;
                        return;
                    }
                }

                try
                {
                    foreach (var file in System.IO.Directory.EnumerateFiles(Directory, "*", SearchOption.TopDirectoryOnly))
                        Files.Add(file);
                    
                    IsDirectoryValid = true;
                }
                catch (Exception)
                {
                    IsDirectoryValid = false;
                    return;
                }
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
                        if (dialog.FileNames.Where(file => !File.Exists(file))?.Count() > 0)
                            return;
                    }
                    catch(IOException)
                    {
                        return;
                    }

                    foreach (var file in dialog.FileNames)
                    {
                        if (CheckIsRequiredAdministratorPrivileges(file))
                        {
                            if(!RequiredAdministratorForThisFiles())
                                return;
                        }

                        Files.Add(file);
                    }
                }
            }
        }

        public void DeleteSelectedFile()
        {
            if (Files.Contains(SelectedFile))
                Files.Remove(SelectedFile);
        }

        private bool CheckIsRequiredAdministratorPrivileges(string path)
        {
            var startsWithPaths = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.AdminTools),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonAdminTools),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                Environment.GetFolderPath(Environment.SpecialFolder.SystemX86),
                Environment.GetFolderPath(Environment.SpecialFolder.Windows)
            };

            return startsWithPaths
                .Where(searched => path.StartsWith(searched, StringComparison.InvariantCultureIgnoreCase))
                .Count() > 0;
        }

        public void ClearList() => Files.Clear();

        #endregion
    }
}
