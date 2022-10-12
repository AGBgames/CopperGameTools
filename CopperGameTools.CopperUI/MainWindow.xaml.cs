using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using CopperGameTools.Builder;
using System.Windows.Controls;

namespace CopperGameTools.CopperUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileInfo? CurrentFile { get; set; }
        public DirectoryInfo? CurrentFileDir { get; set; }
        public CGTProjBuilder? ProjectBuilder { get; set; }
        public string EditorNoPKFOpenedText { get; }
        public string EditorDefaultLogStart { get; }
        public string EditorDefaultLogPrefix { get; }
        TreeViewItem SourceFiles { get; set; }
        TreeViewItem PKFKeys { get; set; }
        TreeViewItem AssetFiles { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            EditorNoPKFOpenedText = "PLEASE OPEN A PKF-FILE";
            EditorDefaultLogStart = $"Log Start.\n";
            EditorDefaultLogPrefix = $"{DateTime.Now} ->";
            SourceFiles = new TreeViewItem() { Header = "Source Files" };
            PKFKeys = new TreeViewItem() { Header = "PKF Keys" };
            AssetFiles = new TreeViewItem() { Header = "Asset Files" };

            PostStartup();
        }

        // ------------------------------ Util Methods ------------------------------ \\

        private void PostStartup()
        {
            DisableEditor();
        }

        private void DisableEditor()
        {
            editor.Text = "PLEASE OPEN A PKF-FILE";
            editor.IsEnabled = false;
        }

        private void EnableEditor()
        {
            editor.IsEnabled = true;
        }
        
        private void ToogleOnPKFLoadedButtons()
        {
            SaveMenuItem.IsEnabled = !SaveMenuItem.IsEnabled;
            UnloadMenuItem.IsEnabled = !UnloadMenuItem.IsEnabled;
            BuildProjectMenuItem.IsEnabled = !BuildProjectMenuItem.IsEnabled;
        }

        private void RefreshOutliner()
        {
            // keys
            foreach (var key in ProjectBuilder?.ProjFile.FileKeys)
            {
                PKFKeys.Items.Add(key.Key);
            }

            // files
            var srcFileDir = ProjectBuilder.ProjFile.KeyGet("src");
            foreach(var file in Directory.GetFiles(srcFileDir, "*.js", SearchOption.AllDirectories))
            {
                SourceFiles.Items.Add(file);
            }
        }

        private void PostLoad()
        {
            Log($"Opening {CurrentFile?.Name}");
            EnableEditor();
            ToogleOnPKFLoadedButtons();
            if (File.Exists(CurrentFile?.DirectoryName + "/copperui/latest_log.txt")) {
                Log($"Restoring log from last session with {CurrentFile?.Name}");
                Log(File.ReadAllText(CurrentFile?.DirectoryName + "/copperui/latest_log.txt"));
            }

            outline.Items.Add(SourceFiles);
            outline.Items.Add(PKFKeys);
            outline.Items.Add(AssetFiles);
            RefreshOutliner();
        }

        private void PostUnload()
        {
            Log($"Unloading {CurrentFile?.Name}");
            DisableEditor();
            ToogleOnPKFLoadedButtons();
            CurrentFile = null;
            ProjectBuilder = null;
            outline.Items.Clear();
        }

        private void SaveCurrentFile()
        {
            if (CurrentFile == null)
            {
                MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                    MessageBoxButton.OK);
                return;
            }

            File.WriteAllText(CurrentFile.FullName, editor.Text);
            // MessageBox.Show("File saved.", "Copper Game Tools UI");
            Log($"Saving {CurrentFile.FullName}");
            RefreshOutliner();
        }

        private void LoadCurrentFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.DefaultExt = ".pkf";
            dialog.Filter += "PKF Files (*.pkf)|*.pkf";

            if ((bool) dialog.ShowDialog() == false) return;

            try
            {
                CurrentFile = new FileInfo(dialog.FileName);
                CurrentFileDir = new DirectoryInfo(path: CurrentFile.DirectoryName);
                ProjectBuilder = new CGTProjBuilder(new CGTProjFile(CurrentFile));
            }
            catch (System.Exception)
            {
                MessageBox.Show("Failed to load PKF-File!", "Copper Game Tools UI", MessageBoxButton.OK, MessageBoxImage.None);
                throw;
            }

            Title = $"Copper Game Tools UI | {CurrentFile.Name}";

            editor.Text = File.ReadAllText(CurrentFile.FullName);

            PostLoad();
        }
        
        private void Log(string text)
        {
            log.AppendText($"{EditorDefaultLogPrefix} {text}\n");
        }

        // ------------------------------ MenuItem Click Event Handlers ------------------------------ \\

        public void LoadClickEvent(Object sender, RoutedEventArgs e)
        {
            LoadCurrentFile();
        }

        public void SaveClickEvent(Object sender, RoutedEventArgs e)
        {
            SaveCurrentFile();
        }

        public void UnloadClickEvent(Object sender, RoutedEventArgs e)
        {
            if (CurrentFile == null)
            {
                MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                    MessageBoxButton.OK);
                return;
            }

            Title = $"Copper Game Tools UI";
            PostUnload();
        }

        public void QuitClickEvent(Object sender, RoutedEventArgs e)
        {
            if (CurrentFile == null)
            {
                this.Close();
            }
            else
            {
                switch (MessageBox.Show($"Save {CurrentFile.Name} before closing? (Unsaved changes will be lost!)", 
                    "Copper Game Tools UI", 
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        SaveCurrentFile();
                        this.Close();
                        break;
                    case MessageBoxResult.No:
                        this.Close();
                        break;
                }    
            }
        }

        public void BuildProjectClickEvent(Object sender, RoutedEventArgs e)
        {
            
        }

        public void SaveLogClickEvent(Object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(CurrentFileDir?.FullName + "/copperui/");
            File.WriteAllText($"{CurrentFileDir?.FullName}/copperui/latest_log.txt", 
                log.Text);
            Log($"Saved log to {CurrentFileDir?.FullName}/copperui/latest_log.txt");
        }

        public void ClearLogClickEvent(Object sender, RoutedEventArgs e)
        {
            log.Clear();
        }

        public void AutoSaveLogClickEvent(Object sender, RoutedEventArgs e)
        {
            if (AutoSaveLogMenuItem.IsChecked)
            {
                AutoSaveLogMenuItem.IsChecked = false;
                Log("Disabled Log-AutoSave for the current project.");
            }
            else
            { 
                AutoSaveLogMenuItem.IsChecked = true;
                Log("Enabled Log-AutoSave for the current project.");
            }
        }

        public void EditorTextChanged(Object sender, TextChangedEventArgs e)
        {
            RefreshOutliner();
        }
    }
}
