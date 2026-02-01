using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NotesApp.Models;
using NotesApp.Services;

namespace NotesApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly NoteStorageService _storageService;
        private List<Note> _allNotes = new();

        [ObservableProperty]
        private ObservableCollection<YearGroup> _yearGroups = new();

        [ObservableProperty]
        private Note? _selectedNote;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _editorContent = string.Empty;

        [ObservableProperty]
        private bool _isEditing;

        public MainViewModel(NoteStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task LoadAsync()
        {
            _allNotes = await _storageService.LoadNotesAsync();
            RefreshGroups();
            if (_allNotes.Count > 0)
                SelectedNote = _allNotes.OrderByDescending(n => n.LastModifiedDate).First();
        }

        partial void OnSearchTextChanged(string value)
        {
            RefreshGroups();
        }

        partial void OnSelectedNoteChanged(Note? value)
        {
            if (value != null)
                EditorContent = value.Content;
            else
                EditorContent = string.Empty;
        }

        [RelayCommand]
        private async Task AddNote()
        {
            var note = new Note
            {
                Content = string.Empty,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };
            _allNotes.Insert(0, note);
            RefreshGroups();
            SelectedNote = note;
            await SaveAsync();
        }

        [RelayCommand]
        private async Task DeleteNote()
        {
            if (SelectedNote == null) return;
            _allNotes.Remove(SelectedNote);
            RefreshGroups();
            SelectedNote = _allNotes.OrderByDescending(n => n.LastModifiedDate).FirstOrDefault();
            await SaveAsync();
        }

        [RelayCommand]
        private async Task ExportNotes()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON 文件|*.json",
                FileName = $"Notes_backup_{DateTime.Now:yyyyMMdd}",
                Title = "导出备忘录"
            };
            if (dialog.ShowDialog() == true)
            {
                await _storageService.ExportNotesAsync(_allNotes, dialog.FileName);
                System.Windows.MessageBox.Show($"已导出 {_allNotes.Count} 条备忘录", "导出成功",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task ImportNotes()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON 文件|*.json",
                Title = "导入备忘录"
            };
            if (dialog.ShowDialog() == true)
            {
                var imported = await _storageService.ImportNotesAsync(dialog.FileName);
                if (imported.Count == 0) return;

                // Merge: add notes with new IDs that don't already exist
                var existingIds = _allNotes.Select(n => n.Id).ToHashSet();
                int added = 0;
                foreach (var note in imported)
                {
                    if (!existingIds.Contains(note.Id))
                    {
                        _allNotes.Add(note);
                        added++;
                    }
                }

                if (added > 0)
                {
                    RefreshGroups();
                    await SaveAsync();
                }

                System.Windows.MessageBox.Show($"导入 {added} 条新备忘录（跳过 {imported.Count - added} 条重复）",
                    "导入完成", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        public async Task SaveCurrentNote()
        {
            if (SelectedNote == null) return;
            if (SelectedNote.Content == EditorContent) return;

            SelectedNote.Content = EditorContent;
            SelectedNote.LastModifiedDate = DateTime.Now;
            RefreshGroups();
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            await _storageService.SaveNotesAsync(_allNotes);
        }

        private void RefreshGroups()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allNotes
                : _allNotes.Where(n =>
                    n.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

            var groups = filtered
                .GroupBy(n => n.Year)
                .OrderByDescending(g => g.Key)
                .Select(g => new YearGroup
                {
                    Year = g.Key,
                    Notes = new ObservableCollection<Note>(
                        g.OrderByDescending(n => n.LastModifiedDate))
                });

            YearGroups = new ObservableCollection<YearGroup>(groups);

            if (SelectedNote != null && !filtered.Contains(SelectedNote))
                SelectedNote = filtered.OrderByDescending(n => n.LastModifiedDate).FirstOrDefault();
        }
    }
}
