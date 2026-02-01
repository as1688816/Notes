# Notes

A clean, Apple-style memo application for Windows, built with C# WPF.

![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

## Features

- **Auto Year Grouping** — Notes are automatically categorized by year in the sidebar
- **Time Sorting** — Newest notes appear first within each year group
- **Smart Title Formatting** — First line auto-styled as bold large title; body text returns to normal size after line break
- **Keyword Search** — Real-time search filtering across all notes (case-insensitive)
- **Auto Date Stamping** — Modification date is automatically updated when you finish editing
- **Export / Import** — Backup and restore notes as JSON files
- **Local Storage** — Data saved to `AppData\Local\Notes\`, keeping your desktop clean

## Screenshot

```
┌──────────────────────────────────────────┐
│ [🔍 Search]          [+] [🗑] [⬇] [⬆]  │
├─────────────┬────────────────────────────┤
│  2026       │                            │
│   Note 1    │  Title (Bold & Large)      │
│   Note 2    │                            │
│  2025       │  Body text here...         │
│   Note 3    │                            │
│             │            2026/02/01 10:30│
└─────────────┴────────────────────────────┘
```

## Download

Go to [Releases](https://github.com/as1688816/Notes/releases) and download `Notes.exe`.

- **Standalone version** (~63MB): No dependencies required, runs on any Windows PC.

## Build from Source

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Steps

```bash
git clone https://github.com/as1688816/Notes.git
cd Notes
dotnet build
dotnet run
```

### Publish

```bash
# Lightweight (requires .NET runtime on target machine)
dotnet publish -c Release --self-contained false -o ./app

# Standalone single-file EXE (no dependencies)
dotnet publish -c Release -r win-x64 --self-contained \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:EnableCompressionInSingleFile=true \
  -o ./publish
```

## Tech Stack

- **Framework**: C# WPF (.NET 10)
- **MVVM**: CommunityToolkit.Mvvm
- **Storage**: Newtonsoft.Json (local JSON file)

## Project Structure

```
├── Models/Note.cs              # Data model
├── ViewModels/MainViewModel.cs # Core logic (MVVM)
├── Services/NoteStorageService.cs # JSON persistence
├── Converters/Converters.cs    # Value converters
├── Styles/AppStyles.xaml       # Apple-style UI theme
├── MainWindow.xaml / .cs       # Main UI & editor
├── App.xaml / .cs              # App entry point
└── app.ico                     # App icon
```

## Data Location

Notes are stored at:

```
C:\Users\<YourName>\AppData\Local\Notes\notes.json
```
