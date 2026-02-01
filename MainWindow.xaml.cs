using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using NotesApp.ViewModels;

namespace NotesApp;

public partial class MainWindow : Window
{
    private MainViewModel? _vm;
    private bool _suppressTextChanged;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        DataContextChanged += (_, _) =>
        {
            _vm = DataContext as MainViewModel;
            if (_vm != null)
            {
                _vm.PropertyChanged += ViewModel_PropertyChanged;
            }
        };
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vm != null)
            await _vm.LoadAsync();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedNote))
        {
            LoadNoteIntoEditor();
        }
    }

    private void LoadNoteIntoEditor()
    {
        _suppressTextChanged = true;
        Editor.Document.Blocks.Clear();

        if (_vm?.SelectedNote == null)
        {
            _suppressTextChanged = false;
            return;
        }

        var content = _vm.SelectedNote.Content ?? string.Empty;
        if (string.IsNullOrEmpty(content))
        {
            Editor.Document.Blocks.Add(new Paragraph());
            _suppressTextChanged = false;
            Editor.Focus();
            return;
        }

        var lines = content.Split('\n');
        BuildDocument(lines);
        _suppressTextChanged = false;
    }

    private void BuildDocument(string[] lines)
    {
        Editor.Document.Blocks.Clear();

        if (lines.Length == 0) return;

        // Title paragraph (first line)
        var titlePara = new Paragraph
        {
            Margin = new Thickness(0, 0, 0, 4),
            LineHeight = 30
        };
        titlePara.Inlines.Add(new Run(lines[0].TrimEnd('\r'))
        {
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E))
        });
        Editor.Document.Blocks.Add(titlePara);

        // Body paragraphs
        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var para = new Paragraph
                {
                    Margin = new Thickness(0, 0, 0, 2),
                    LineHeight = 22
                };
                para.Inlines.Add(new Run(lines[i].TrimEnd('\r'))
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Normal,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E))
                });
                Editor.Document.Blocks.Add(para);
            }
        }
    }

    private string GetPlainText()
    {
        var lines = new System.Collections.Generic.List<string>();
        foreach (var block in Editor.Document.Blocks)
        {
            if (block is Paragraph para)
            {
                var text = new TextRange(para.ContentStart, para.ContentEnd).Text;
                lines.Add(text);
            }
        }
        return string.Join("\n", lines);
    }

    private void ApplyFormatting()
    {
        var blocks = Editor.Document.Blocks.ToList();
        bool isFirst = true;
        foreach (var block in blocks)
        {
            if (block is Paragraph para)
            {
                if (isFirst)
                {
                    para.Margin = new Thickness(0, 0, 0, 4);
                    para.LineHeight = 30;
                    foreach (var inline in para.Inlines)
                    {
                        if (inline is Run run)
                        {
                            run.FontSize = 22;
                            run.FontWeight = FontWeights.Bold;
                        }
                    }
                    isFirst = false;
                }
                else
                {
                    para.Margin = new Thickness(0, 0, 0, 2);
                    para.LineHeight = 22;
                    foreach (var inline in para.Inlines)
                    {
                        if (inline is Run run)
                        {
                            run.FontSize = 14;
                            run.FontWeight = FontWeights.Normal;
                        }
                    }
                }
            }
        }
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressTextChanged || _vm == null) return;

        _suppressTextChanged = true;

        // Save caret position
        var caretPos = Editor.CaretPosition;

        ApplyFormatting();

        // Restore caret
        Editor.CaretPosition = caretPos;

        // Sync plain text to VM
        _vm.EditorContent = GetPlainText();

        _suppressTextChanged = false;
    }

    private async void Editor_LostFocus(object sender, RoutedEventArgs e)
    {
        if (_vm == null) return;
        _vm.EditorContent = GetPlainText();
        await _vm.SaveCurrentNote();
    }
}
