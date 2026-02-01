using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NotesApp.Models;

namespace NotesApp.Services
{
    public class NoteStorageService
    {
        private readonly string _filePath;

        public NoteStorageService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dataFolder = Path.Combine(appData, "Notes");
            Directory.CreateDirectory(dataFolder);
            _filePath = Path.Combine(dataFolder, "notes.json");
        }

        public async Task<List<Note>> LoadNotesAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Note>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonConvert.DeserializeObject<List<Note>>(json) ?? new List<Note>();
        }

        public async Task SaveNotesAsync(List<Note> notes)
        {
            var json = JsonConvert.SerializeObject(notes, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task ExportNotesAsync(List<Note> notes, string exportPath)
        {
            var json = JsonConvert.SerializeObject(notes, Formatting.Indented);
            await File.WriteAllTextAsync(exportPath, json);
        }

        public async Task<List<Note>> ImportNotesAsync(string importPath)
        {
            var json = await File.ReadAllTextAsync(importPath);
            return JsonConvert.DeserializeObject<List<Note>>(json) ?? new List<Note>();
        }
    }
}
