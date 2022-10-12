using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using CopperGameTools.Builder;
using System.Windows.Controls;

namespace CopperGameTools.CopperUI;

public partial class CGTMainWindow : Window
{
    private FileInfo? CurrentFile { get; set; }
    private DirectoryInfo? CurrentFileDir { get; set; }
    private CGTProjBuilder? ProjectBuilder { get; set; }
    private string EditorDefaultLogPrefix { get; }
    private TreeViewItem SourceFiles { get; set; }
    private TreeViewItem PkfKeys { get; set; }

    public CGTMainWindow()
    {
        InitializeComponent();
        EditorDefaultLogPrefix = $"{DateTime.Now} ->";
        SourceFiles = new TreeViewItem() { Header = "Source Files" };
        PkfKeys = new TreeViewItem() { Header = "PKF Keys" };

        PostStartup();
    }

    // ------------------------------ Util Methods ------------------------------ \\

    private void PostStartup()
    {
        ToggleEditor();
    }

    private void ToggleEditor()
    {
        Editor.IsEnabled = !Editor.IsEnabled;
    }

    private void TogglePkfButtons()
    {
        SaveMenuItem.IsEnabled = !SaveMenuItem.IsEnabled;
        UnloadMenuItem.IsEnabled = !UnloadMenuItem.IsEnabled;
        BuildProjectMenuItem.IsEnabled = !BuildProjectMenuItem.IsEnabled;
        CheckPkfMenuItem.IsEnabled = !CheckPkfMenuItem.IsEnabled;
    }

    private void PrepareOutline()
    {
        if (ProjectBuilder?.ProjFile == null || CurrentFileDir == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                MessageBoxButton.OK);
            return;
        }

        Outline.Items.Clear();
        SourceFiles.Items.Clear();
        PkfKeys.Items.Clear();

        Outline.Items.Add(SourceFiles);
        Outline.Items.Add(PkfKeys);

        ProjectBuilder.ProjFile.ReloadKeys();

        foreach (var key in ProjectBuilder.ProjFile.FileKeys)
        {
            PkfKeys.Items.Add(key.Key);
        }

        var srcFileDir = ProjectBuilder.ProjFile.KeyGet("src");
        foreach (var file in Directory.GetFiles($"{CurrentFileDir.FullName}/{srcFileDir}", "*.js",
                     SearchOption.AllDirectories))
        {
            // i just the fucking parent folder from /src/ on, pls fucking god.
            var info = new FileInfo(file);
            var name = info.FullName.Split(@"\");
            SourceFiles.Items.Add($"{name.Last()}");
        }
    }

    private void CheckPkfFile()
    {
        if (ProjectBuilder == null) return;
        Errors.Clear();

        var check = ProjectBuilder.ProjFile.FileCheck();

        if (check.ResultType == CGTProjFileCheckResultType.Errors)
        {
            foreach (var err in check.ResultErrors)
            {
                Errors.AppendText($"{err.ErrorText} ==> {err.ErrorType} ({err.IsCritical})\n");
            }
        }
        else
        {
            Errors.Text = "No Problems found.";
        }
    }

    private void PostLoad()
    {
        Log($"Opening {CurrentFile?.Name}");
        ToggleEditor();
        TogglePkfButtons();
        if (File.Exists(CurrentFile?.DirectoryName + "/copperui/latest_log.txt"))
        {
            switch (MessageBox.Show("Load latest saved LogBox?", "Copper Game Tools UI", MessageBoxButton.YesNo,
                        MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    Log(File.ReadAllText(CurrentFile?.DirectoryName + "/copperui/latest_log.txt"));
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        CheckPkfFile();
        PrepareOutline();
    }

    private void PostUnload()
    {
        Log($"Unloading {CurrentFile?.Name}");
        ToggleEditor();
        TogglePkfButtons();
        CurrentFile = null;
        ProjectBuilder = null;
        Outline.Items.Clear();
        Editor.Clear();
        LogBox.Clear();
    }

    private void SaveCurrentFile()
    {
        if (CurrentFile == null)
        {
            MessageBox.Show("Please open a Project Key File (*.pkf) first.", "Copper Game Tools UI",
                MessageBoxButton.OK);
            return;
        }

        File.WriteAllText(CurrentFile.FullName, Editor.Text);
        Log($"Saving {CurrentFile.FullName}");
        CheckPkfFile();
        PrepareOutline();
    }

    private void LoadCurrentFile()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Multiselect = false;
        dialog.DefaultExt = ".pkf";
        dialog.Filter += "PKF Files (*.pkf)|*.pkf";

        if ((bool)dialog.ShowDialog() == false) return;

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

        Title = $"Copper Game Tools UI | {CurrentFile.Name}";

        Editor.Text = File.ReadAllText(CurrentFile.FullName);

        PostLoad();
    }

    private void Log(string text)
    {
        LogBox.AppendText($"{EditorDefaultLogPrefix} {text}\n");
    }

    // ------------------------------ MenuItem Click Event Handlers ------------------------------ \\

    private void LoadClickEvent(object sender, RoutedEventArgs e)
    {
        LoadCurrentFile();
    }

    private void SaveClickEvent(object sender, RoutedEventArgs e)
    {
        SaveCurrentFile();
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
                    SaveCurrentFile();
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
    }

    private void CheckPkfClickEvent(object sender, RoutedEventArgs e)
    {
        CheckPkfFile();
    }

    private void SaveLogClickEvent(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(CurrentFileDir?.FullName + "/copperui/");
        File.WriteAllText($"{CurrentFileDir?.FullName}/copperui/latest_log.txt",
            LogBox.Text);
        Log($"Saved LogBox to {CurrentFileDir?.FullName}/copperui/latest_log.txt");
    }

    private void ClearLogClickEvent(object sender, RoutedEventArgs e)
    {
        LogBox.Clear();
    }
}