using System.Collections.Generic;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using FileManipulator.ViewModels;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy FilesSelectorControl.xaml
    /// </summary>
    public partial class FilesSelectorView : UserControl
    {
        public FilesSelectorView()
        {
            InitializeComponent();

            this.haveAdministratorPrivileges = CheckAreAdministratorPrivileges();
        }

        private bool haveAdministratorPrivileges;

        private bool CheckAreAdministratorPrivileges()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private bool WhenRequiredAdministratorPrivileges()
        {
            if (this.haveAdministratorPrivileges)
                return true;

            MessageBox.Show("Operowanie na plikach z tego folderu wymaga uprawnień administratora.", "Manipulator", MessageBoxButton.OK, MessageBoxImage.Warning);

            return false;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is FilesSelectorViewModel model)
                model.RequiredAdministratorForThisFiles = WhenRequiredAdministratorPrivileges;
        }
    }
}
