using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CopperGameTools.CopperUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileInfo? CurrentFile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            editor.Text = "PLEASE OPEN A PKF-FILE";
            editor.IsEnabled = false;
        }

        public void LoadClickEvent(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.DefaultExt = ".pkf";
            dialog.Filter += "PKF Files (*.pkf)|*.pkf";

            if (!(bool)dialog.ShowDialog()) return;

            try
            {
                 CurrentFile = new FileInfo(dialog.FileName);
            }
            catch (System.Exception)
            {
                MessageBox.Show("Failed to load PKF-File!", "Copper Game Tools UI", MessageBoxButton.OK, MessageBoxImage.None);
                throw;
            }

            Title = $"Copper Game Tools UI | {CurrentFile.Name}";
            SaveMenuItem.IsEnabled = true;
            UnloadMenuItem.IsEnabled = true;
            editor.Text = File.ReadAllText(CurrentFile.FullName);
            editor.IsEnabled = true;
        }

        public void SaveClickEvent(Object sender, RoutedEventArgs e)
        {
            if (CurrentFile == null)
            {
                MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                    MessageBoxButton.OK);
                return;
            }

            File.WriteAllText(CurrentFile.FullName, editor.Text);
            MessageBox.Show("File saved.", "Copper Game Tools UI");
        }

        public void UnloadClickEvent(Object sender, RoutedEventArgs e)
        {
            if (CurrentFile == null)
            {
                MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                    MessageBoxButton.OK);
                return;
            }

            CurrentFile = null;
            Title = $"Copper Game Tools UI";
            editor.Text = "PLEASE OPEN A PKF-FILE";
            editor.IsEnabled = false;
            SaveMenuItem.IsEnabled = false;
            UnloadMenuItem.IsEnabled = false;
        }

        public void QuitClickEvent(Object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Save PKF-File (when opened)?", "Copper Game Tools", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    if (CurrentFile == null)
                    {
                        MessageBox.Show("No PKF-File opened, closing without saving..", "Copper Game Tools UI",
                        MessageBoxButton.OK);
                        return;
                    }
                    break;
                case MessageBoxResult.No:
                    this.Close();
                    break;
                default:
                    break;
            }
        }
    }
}
