using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace CopperGameTools.CopperUI;

public partial class KeyChangeInputBox : Window
{
    private string OldKeyName { get; }
    private string OldKeyValue { get; }
    private TextBox Editor { get; }

    public KeyChangeInputBox(string oldKeyName, string oldKeyValue, TextBox editor)
    {
        InitializeComponent();

        OldKeyName = oldKeyName;
        OldKeyValue = oldKeyValue;
        Editor = editor;

        NewKeyName.Text = OldKeyName;
        NewKeyValue.Text = OldKeyValue;
    }

    public CheckBox IsAddNew => this.AddNew;

    private void OKButtonClickEvent(object sender, RoutedEventArgs e)
    {
        UpdateKey();
        Close();
    }


    private void UpdateKey()
    {
        if (AddNew.IsChecked == null) return;
        if ((bool)AddNew.IsChecked == true)
        {
            Editor.Text += $"\n{NewKeyName.Text}={NewKeyValue.Text}";           
        }
        else {
            Editor.Text = Editor.Text.Replace($"{OldKeyName}={OldKeyValue}", $"{NewKeyName.Text}={NewKeyValue.Text}");
        }
    }
}