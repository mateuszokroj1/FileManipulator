using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace FileManipulator.UI
{
    public partial class App : Application
    {
        #region Constructor

        public App()
        {
#if DEBUG
#else
            DispatcherUnhandledException += Application_OnDispatcherUnhandledException;
#endif
        }

        #endregion

        #region Methods

        private void Application_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var time = DateTime.Now;
            var filename = $"filemanipulator_{time.Year}{time.Month}{time.Day}{time.Hour}{time.Minute}{time.Second}{time.Millisecond}.log";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), filename);

            try
            {
                using (var logStream = File.CreateText(path))
                {
                    logStream.WriteLine($"Exception type: {e.Exception.GetType().FullName}");
                    logStream.WriteLine("Message:");
                    logStream.WriteLine(e.Exception.Message);
                    logStream.WriteLine("Stack trace:");
                    logStream.Write(e.Exception.StackTrace);
                    logStream.Flush();
                }
            }
            catch(IOException)
            { } // Abort operation and continue

            MessageBox.Show
            (
                string.Format(Messages.Nieoczekiwany_blad1, filename),
                "File Manipulator",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            Shutdown();
        }

        #endregion
    }
}
