using System.Windows.Controls;

using FileManipulator.ViewModels;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy ManipulatorView.xaml
    /// </summary>
    public partial class ManipulatorView : UserControl
    {
        public ManipulatorView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if(DataContext is ManipulatorViewModel model)
            {
                model.GetDirectoryFromDialog = input =>
                {
                    using (var dialog = new CommonOpenFileDialog())
                    {
                        dialog.IsFolderPicker = true;
                        dialog.ShowHiddenItems = false;
                        dialog.AllowNonFileSystemItems = false;
                        dialog.Title = "Wybierz miejsce docelowe";
                        dialog.InitialDirectory = input;
                        dialog.EnsureReadOnly = false;
                        dialog.Multiselect = false;
                        dialog.NavigateToShortcut = true;

                        if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                            return input;

                        return dialog.FileName;
                    }
                };
            }
        }
    }
}
