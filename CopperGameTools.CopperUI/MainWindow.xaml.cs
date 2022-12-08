using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using CopperGameTools.Builder;
using System.Windows.Controls;
using System.Windows.Input;
using CopperGameTools.CopperUI.FeatureWindows;

namespace CopperGameTools.CopperUI;

public partial class CGTMainWindow : Window
{
    private FileInfo? CurrentFile { get; set; }
    private DirectoryInfo? CurrentFileDir { get; set; }
    private CGTProjBuilder? ProjectBuilder { get; set; }
    
    private TreeViewItem KeysFromFile { get; }
    private TreeViewItem AssetFiles { get; }

    public ICommand LoadPkfFileCommand { get; }
    public ICommand SavePkfFileCommand { get; }
    public ICommand RevealPkfFileCommand { get; }
    public ICommand FastEditKeyCommand { get; }
    public ICommand BuildProjectCommand { get; }
        
    public CGTMainWindow()
    {
        InitializeComponent();
        KeysFromFile = new TreeViewItem() { Header = "Keys (Read From File)" };
        AssetFiles = new TreeViewItem() { Header = "Asset Files" };

        LoadPkfFileCommand = new CGTActionCommand(() =>
        {
            PostUnload();
            LoadPkfFile();
        });
        SavePkfFileCommand = new CGTActionCommand(() =>
        {
            SavePkfFile(false);
        });
        RevealPkfFileCommand = new CGTActionCommand(() =>
        {
            RevealInExplorer();
        });
        FastEditKeyCommand = new CGTActionCommand(() => {
            FastEditKey();
        });
        BuildProjectCommand = new CGTActionCommand(() => {
            BuildProject();
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
        RevealInExplorerMenuItem.IsEnabled = !RevealInExplorerMenuItem.IsEnabled;
        FastEditKeyMenuItem.IsEnabled = !FastEditKeyMenuItem.IsEnabled;
    }

    /**
     * Reloads the Project-Outliner (the TreeView on the left)
     * Shows the saved Keys, Asset Files and more.
     * It also offers tools like Fast Edit Key.
     */
    private void PrepareOutline()
    {
        if (ProjectBuilder?.ProjFile == null || CurrentFileDir == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "CopperUI",
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
            keyToAdd.MouseLeftButtonUp += FastEditKeyEvent;
            KeysFromFile.Items.Add(keyToAdd);
        }
        CheckPkfFile();
    }
    
    private void FastEditKeyEvent(object sender, MouseButtonEventArgs e)
    {
        var item = sender as TreeViewItem;

        if (item?.Header.ToString() == null) return;
        
        var keyValue = ProjectBuilder?.ProjFile.KeyGet(item?.Header.ToString());
        var keyName = item?.Header.ToString();
        if (keyValue == null || keyName == null) return;

        var dialog = new KeyChangeInputBox(keyName, keyValue, Editor);
        dialog.Show();
    }

    // Checks the PKF File for problems using the FileCheck method from CGTProjBuilder.
    private void CheckPkfFile()
    {
        if (ProjectBuilder == null) return;
        Errors.Clear();

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

    // Opens the file exlorer in the directory of the pkf.
    private void RevealInExplorer()
    {
        var proc = new Process();
        proc.StartInfo.FileName = "explorer.exe";
        proc.StartInfo.Arguments += $"{CurrentFileDir?.FullName}";
        proc.Start();
        proc.WaitForExit();
    }
    
    private void FastEditKey()
    {
        var dialog = new KeyChangeInputBox("", "", Editor);
        dialog.IsAddNew.IsChecked = true;
        dialog.Show();
    }

    private void BuildProject()
    {
        if (ProjectBuilder == null) return;

        var build = ProjectBuilder?.Build();

        switch (build?.ResultType)
        {
            case CGTProjBuilderResultType.DoneNoErrors:
                break;
            case CGTProjBuilderResultType.DoneWithErrors:
                break;
            case CGTProjBuilderResultType.FailedWithErrors:
                MessageBox.Show("Build failed with errors.");
                break;
        }
    }

    /** ------------------------------ Post Action Methods ------------------------------ */

    // Defines what should happen after the start (outside of the constructor).
    private void PostStartup()
    {
        ToggleEditor();
    }

    // Defines what should happen after loading a PKF.
    private void PostLoad()
    {
        ToggleEditor();
        TogglePkfButtons();
        CheckPkfFile();
        PrepareOutline();
    }

    // Defines what should happen after unloading a PKF.
    private void PostUnload()
    {
        ToggleEditor();
        TogglePkfButtons();
        CurrentFile = null;
        ProjectBuilder = null;
        Outline.Items.Clear();
        Editor.Clear();
    }

    /** ------------------------------ Action Methods ------------------------------ */
    
    // Saves the current PKF.
    private void SavePkfFile(bool silent)
    {
        if (CurrentFile == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "CopperUI",
                MessageBoxButton.OK);
            return;
        }

        File.WriteAllText(CurrentFile.FullName, Editor.Text);
        CheckPkfFile();
        PrepareOutline();
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

        if (dialog == null) return;
        if (dialog?.ShowDialog() == false) return;
        if (dialog?.FileName == null) return;

        try
        {
            CurrentFile = new FileInfo(dialog.FileName);
            if (CurrentFile.DirectoryName != null)
                CurrentFileDir = new DirectoryInfo(path: CurrentFile.DirectoryName);
            ProjectBuilder = new CGTProjBuilder(new CGTProjFile(CurrentFile));
        }
        catch (System.Exception)
        {
            MessageBox.Show("Failed to load PKF-File!", "CopperUI", MessageBoxButton.OK,
                MessageBoxImage.None);
            throw;
        }

        Title = $"CopperUI | {CurrentFile.Name}";

        Editor.Text = File.ReadAllText(CurrentFile.FullName);

        PostLoad();
    }

    /** ------------------------------ MenuItem Click Event Handlers ------------------------------ */

    private void LoadClickEvent(object sender, RoutedEventArgs e)
    {
        LoadPkfFile();
    }

    private void SaveClickEvent(object sender, RoutedEventArgs e)
    {
        SavePkfFile(false);
    }

    private void UnloadClickEvent(object sender, RoutedEventArgs e)
    {
        if (CurrentFile == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "CopperUI",
                MessageBoxButton.OK);
            return;
        }

        Title = $"CopperUI";
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
                        "CopperUI",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    SavePkfFile(false);
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
        BuildProject();
    }

    private void AboutMenuItemClickEvent(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"CopperUI aka. CopperUI v0.1.2 made by Nils 'AGBDev' Boehm. \n" +
                        $"This Software is only to be used by licensed employees from AGBgames.\n" +
                        $"It is not to be shared outside of AGBgames.", 
            "CopperUI",
            MessageBoxButton.OK,
            MessageBoxImage.Asterisk);
    }

    private void RevealInExplorerClickEvent(object sender, RoutedEventArgs e)
    {
        RevealInExplorer();
    }

    private void FastEditKeyClickEvent(object sender, RoutedEventArgs e)
    {
        FastEditKey();
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CurrentFile == null) return;
        SavePkfFile(true);
        PrepareOutline();
    }

    private void OptionsClickEvent(object sender, RoutedEventArgs e)
    {
        var options = new Settings();
        options.Show();
    }
}