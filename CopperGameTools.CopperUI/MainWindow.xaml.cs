using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using CopperGameTools.Builder;
using System.Windows.Controls;
using System.Windows.Input;

namespace CopperGameTools.CopperUI;

public partial class CGTMainWindow : Window
{
    private FileInfo? CurrentFile { get; set; }
    private DirectoryInfo? CurrentFileDir { get; set; }
    private CGTProjBuilder? ProjectBuilder { get; set; }
    private bool CurrentFileHasLog { get; set; }
    
    private TreeViewItem KeysFromFile { get; }
    private TreeViewItem AssetFiles { get; }
    
    private string EditorDefaultLogPrefix { get; }
    private string EditorDefaultLogSavePath { get; }

    public ICommand LoadPkfFileCommand { get; }
    public ICommand SavePkfFileCommand { get; }
    public ICommand RevealPkfFileCommand { get; }
    public ICommand LogSaveCommand { get; }
    public ICommand LogClearCommand { get; }

    public CGTMainWindow()
    {
        InitializeComponent();
        EditorDefaultLogPrefix = $"[{DateTime.Now}]: ";
        EditorDefaultLogSavePath = "/.coppui/log.txt";
        KeysFromFile = new TreeViewItem() { Header = "Keys (Read From File)" };
        AssetFiles = new TreeViewItem() { Header = "Asset Files" };

        LoadPkfFileCommand = new CGTActionCommand(() =>
        {
            PostUnload();
            LoadPkfFile();
        });
        SavePkfFileCommand = new CGTActionCommand(() =>
        {
            SavePkfFile();
        });
        RevealPkfFileCommand = new CGTActionCommand(() =>
        {
            RevealInExplorer();
        });
        LogSaveCommand = new CGTActionCommand(() =>
        {
            SaveLog(true);
        });
        LogClearCommand = new CGTActionCommand(() =>
        {
            ClearLog();
        });

        DataContext = this;

        PostStartup();
    }

    /** ------------------------------ Util Methods ------------------------------ */

    /**
     * Enables / Disables the editor.
     * The Editor is disabled on startup (as default).
    */
    private void ToggleEditor()
    {
        Editor.IsEnabled = !Editor.IsEnabled;
    }

    /** Enables / Disables the buttons that should only be enabled when a PKF-File is loaded.
    // Those buttons are disabled on startup (as default).
    */
    private void TogglePkfButtons()
    {
        SaveMenuItem.IsEnabled = !SaveMenuItem.IsEnabled;
        UnloadMenuItem.IsEnabled = !UnloadMenuItem.IsEnabled;
        BuildProjectMenuItem.IsEnabled = !BuildProjectMenuItem.IsEnabled;
        CheckPkfMenuItem.IsEnabled = !CheckPkfMenuItem.IsEnabled;
        RevealInExplorerMenuItem.IsEnabled = !RevealInExplorerMenuItem.IsEnabled;
    }

    // Reloads the outline (on the left)
    private void PrepareOutline()
    {
        if (ProjectBuilder?.ProjFile == null || CurrentFileDir == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                MessageBoxButton.OK);
            return;
        }

        Outline.Items.Clear();
        KeysFromFile.Items.Clear();
        AssetFiles.Items.Clear();

        Outline.Items.Add(KeysFromFile);
        Outline.Items.Add(AssetFiles);

        ProjectBuilder.ProjFile.ReloadKeys();

        foreach (var key in ProjectBuilder.ProjFile.FileKeys)
        {
            var keyToAdd = new TreeViewItem() { Header = $"{key.Key}" };
            keyToAdd.MouseLeftButtonUp += TreeItemMouseLeftUpEvent;
            KeysFromFile.Items.Add(keyToAdd);
        }
        CheckPkfFile();
    }
    
    private void TreeItemMouseLeftUpEvent(object sender, MouseButtonEventArgs e)
    {
        if (sender == null) return;

        var item = sender as TreeViewItem;
        var keyName = item?.Header.ToString();
        var keyValue = ProjectBuilder?.ProjFile.KeyGet(item?.Header.ToString());
        if (keyValue == null || keyName == null) return;
        var dialog = new KeyChangeInputBox(keyName, keyValue, Editor);
        dialog.Show();
    }

    // Checks the PKF File for problems using the FileCheck method from CGTProjBuilder.
    private void CheckPkfFile()
    {
        if (ProjectBuilder == null) return;
        Errors.Clear();

        // checks file.
        CGTProjFileCheckResult check = ProjectBuilder.ProjFile.FileCheck();

        switch (check.ResultType)
        {
            case CGTProjFileCheckResultType.Errors:
            {
                foreach (var err in check.ResultErrors)
                {
                    Errors.AppendText($"{err.ErrorText} | Type => {err.ErrorType} | Is Critical => {err.IsCritical}\n");
                }

                break;
            }
            case CGTProjFileCheckResultType.NoErrors:
                Errors.Text = "No Problems found.";
                break;
        }
    }

    // Appends a text to the LogBox.
    private void Log(string text)
    {
        LogBox.AppendText($"{EditorDefaultLogPrefix} {text}\n");
    }
    private void Log(string text, bool usePrefix)
    {
        switch (usePrefix)
        {
            case true:
                LogBox.AppendText($"{EditorDefaultLogPrefix} {text}\n");
                break;
            case false:
                LogBox.AppendText($"{text}\n");
                break;
        }
    }

    private void RevealInExplorer()
    {
        var proc = new Process();
        proc.StartInfo.FileName = "explorer.exe";
        proc.StartInfo.Arguments += $"{CurrentFileDir?.FullName}";
        proc.Start();
        proc.WaitForExit();
    }

    /** ------------------------------ Post Action Methods ------------------------------ */

    // Defines what should happen after the start (outside of the constructor).
    private void PostStartup()
    {
        ToggleEditor();

        LoadPkfFile();
    }

    // Defines what should happen after loading a PKF.
    private void PostLoad()
    {
        Log($"Opening {CurrentFile?.Name}");
        ToggleEditor();
        TogglePkfButtons();
        if (CurrentFileHasLog)
        {
            LogBox.Clear();
            Log(File.ReadAllText($"{CurrentFileDir?.FullName}{EditorDefaultLogSavePath}"), false);
        }
        CheckPkfFile();
        PrepareOutline();
    }

    // Defines what should happen after unloading a PKF.
    private void PostUnload()
    {
        Log($"Unloading {CurrentFile?.Name}");
        ToggleEditor();
        TogglePkfButtons();
        CurrentFile = null;
        ProjectBuilder = null;
        CurrentFileHasLog = false;
        Outline.Items.Clear();
        Editor.Clear();
        LogBox.Clear();
    }

    /** ------------------------------ Action Methods ------------------------------ */
    
    // Saves the current PKF.
    private void SavePkfFile()
    {
        if (CurrentFile == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                MessageBoxButton.OK);
            return;
        }

        File.WriteAllText(CurrentFile.FullName, Editor.Text);
        Log($"Saving {CurrentFile.FullName}");
        if (CurrentFileHasLog)
        {
            File.WriteAllText(CurrentFileDir?.FullName + EditorDefaultLogSavePath, LogBox.Text);
        }
        CheckPkfFile();
        PrepareOutline();
        SaveLog(true);
    }

    // Loads a PKF
    private void LoadPkfFile()
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            DefaultExt = ".pkf"
        };
        dialog.Filter += "PKF Files (*.pkf)|*.pkf";

        if (dialog?.ShowDialog() == false) return;

        try
        {
            CurrentFile = new FileInfo(dialog.FileName);
            if (CurrentFile.DirectoryName != null)
                CurrentFileDir = new DirectoryInfo(path: CurrentFile.DirectoryName);
            ProjectBuilder = new CGTProjBuilder(new CGTProjFile(CurrentFile));
        }
        catch (System.Exception)
        {
            MessageBox.Show("Failed to load PKF-File!", "Copper Game Tools UI", MessageBoxButton.OK,
                MessageBoxImage.None);
            throw;
        }

        CurrentFileHasLog = File.Exists($"{CurrentFileDir?.FullName}{EditorDefaultLogSavePath}");

        Title = $"Copper Game Tools UI | {CurrentFile.Name}";

        Editor.Text = File.ReadAllText(CurrentFile.FullName);

        PostLoad();
    }
    
    // Saves the LogBox
    private void SaveLog(bool isSilent)
    {
        Directory.CreateDirectory(CurrentFileDir?.FullName + "/.coppui/");
        if (!isSilent) Log($"Saved LogBox to {CurrentFileDir?.FullName}{EditorDefaultLogSavePath}");
        File.WriteAllText($"{CurrentFileDir?.FullName}{EditorDefaultLogSavePath}",
            LogBox.Text);
    }
    
    // Clears the log
    private void ClearLog()
    {
        LogBox.Clear();
    }

    /** ------------------------------ MenuItem Click Event Handlers ------------------------------ */

    private void LoadClickEvent(object sender, RoutedEventArgs e)
    {
        LoadPkfFile();
    }

    private void SaveClickEvent(object sender, RoutedEventArgs e)
    {
        SavePkfFile();
    }

    private void UnloadClickEvent(object sender, RoutedEventArgs e)
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

    private void QuitClickEvent(object sender, RoutedEventArgs e)
    {
        if (CurrentFile == null)
        {
            this.Close();
        }
        else
        {
            if (File.ReadAllText(CurrentFile.FullName) == Editor.Text)
            {
                this.Close();
                return;
            }

            switch (MessageBox.Show($"Save {CurrentFile.Name} before closing? (Unsaved changes will be lost!)",
                        "Copper Game Tools UI",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    SavePkfFile();
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    this.Close();
                    break;
            }
        }
    }

    private void BuildProjectClickEvent(object sender, RoutedEventArgs e)
    {
        Log("Building project...");
        MessageBox.Show("Please do not build the project.");
    }

    private void CheckPkfClickEvent(object sender, RoutedEventArgs e)
    {
        CheckPkfFile();
    }

    private void SaveLogClickEvent(object sender, RoutedEventArgs e)
    {
        SaveLog(true);
    }

    private void ClearLogClickEvent(object sender, RoutedEventArgs e)
    {
        ClearLog();
    }

    private void AboutMenuItemClickEvent(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Copper Game Tools UI aka. CopperUI v{Assembly.GetExecutingAssembly().GetName().Version} made by Nils 'AGBDev' Boehm. \n" +
                        $"This Software is only to be used by licensed employees from AGBgames.\n" +
                        $"It is not to be shared outside of AGBgames.", 
            "Copper Game Tools UI",
            MessageBoxButton.OK,
            MessageBoxImage.Asterisk);
    }

    private void RevealInExplorerClickEvent(object sender, RoutedEventArgs e)
    {
        RevealInExplorer();
    }
}