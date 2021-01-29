using System.Text;
using System.Windows;
using System.Windows.Controls;

using FileManipulator.ViewModels;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy WatcherView.xaml
    /// </summary>
    public partial class WatcherView : UserControl
    {
        public WatcherView()
        {
            InitializeComponent();
        }

        private void Root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is WatcherViewModel model)
            {
                model.OnInvalidInput = () => MessageBox.Show("Podano niepoprawną ścieżkę.", "Watcher", MessageBoxButton.OK, MessageBoxImage.Warning);

                model.OnError = exc =>
                {
                    using (var dialog = new TaskDialog())
                    {
                        dialog.Cancelable = false;
                        dialog.Caption = "Watcher";
                        dialog.Text = "Wystąpił błąd podczas wykonywania zadania dlatego zostało przerwane.";
                        dialog.Icon = TaskDialogStandardIcon.Error;
                        dialog.DetailsExpanded = false;
                        dialog.DetailsCollapsedLabel = dialog.DetailsExpandedLabel = "Więcej informacji";
                        dialog.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandContent;
                        dialog.StartupLocation = TaskDialogStartupLocation.CenterScreen;
                        dialog.StandardButtons = TaskDialogStandardButtons.Ok;

                        var builder = new StringBuilder();

                        builder.AppendLine(exc.GetType().FullName);
                        builder.AppendLine($"Message: {exc.Message}");
                        builder.AppendLine($"Stacktrace:");
                        builder.Append(exc.StackTrace);

                        dialog.DetailsExpandedText = builder.ToString();

                        dialog.Show();
                    }
                };
            }
        }
    }
}
