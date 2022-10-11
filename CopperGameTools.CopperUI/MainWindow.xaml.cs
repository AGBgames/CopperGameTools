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
        }

        public void LoadClickEvent(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.DefaultExt = ".pkf";
            dialog.Filter += "PKF Files (*.pkf)|*.pkf";

            if ((bool)!dialog.ShowDialog()) return;

            CurrentFile = new FileInfo(dialog.FileName);

            Title = $"Copper Game Tools UI | {CurrentFile.Name}";
            SaveMenuItem.IsEnabled = true;
        }

        public void SaveClickEvent(Object sender, RoutedEventArgs e)
        {
            if (CurrentFile == null)
            {
                MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                    MessageBoxButton.OK);
                return;
            }
        }
    }
}
