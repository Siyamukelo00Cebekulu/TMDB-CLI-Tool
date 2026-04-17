# TMDB CLI Tool

A simple C# .NET 8.0 command line interface that fetches movie data from The Movie Database (TMDB) API and displays it in the terminal.

## Project Overview

This project demonstrates a clean architecture for a console-based application with clear separation of concerns.
It retrieves movie lists from TMDB for the following categories:

- Popular movies
- Top rated movies
- Upcoming movies
- Now playing movies

The application accepts a single command line argument to choose which category to display.

## Technologies

- C#
- .NET 8.0
- System.Net.Http for HTTP requests
- System.Text.Json for JSON parsing
- Git for version control

## Architectural Pattern

The project uses a layered architecture inspired by clean architecture principles:

- **Presentation layer** (`Presentation/MovieConsoleApp.cs`)
  - Parses CLI arguments
  - Handles console interaction and output
  - Displays usage and errors

- **Service layer** (`Services/MovieService.cs`)
  - Encapsulates application logic for movie retrieval
  - Orchestrates calls to the repository

- **Repository layer** (`Repositories/TmdbMovieRepository.cs`)
  - Handles TMDB API requests
  - Deserializes JSON responses into domain models

- **Domain models** (`Models/`)
  - `Movie.cs`
  - `MoviePage.cs`
  - `MovieType.cs`
  - Defines the data structures used throughout the app

## Project Structure

```
TMDB-CLI-Tool/
├── Models/
│   ├── Movie.cs
│   ├── MoviePage.cs
│   └── MovieType.cs
├── Presentation/
│   └── MovieConsoleApp.cs
├── Repositories/
│   ├── IMovieRepository.cs
│   └── TmdbMovieRepository.cs
├── Services/
│   └── MovieService.cs
├── Program.cs
├── TMDB-CLI-Tool.csproj
└── README.md
```

## Features

- Fetches popular, top rated, upcoming, or now playing movies from TMDB
- Supports command line argument parsing for movie type selection
- Handles missing API keys and API/network failures gracefully
- Outputs movie title, release date, rating, and overview
- Uses a layered architecture for readability and maintainability

## Setup

1. Install the .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Obtain a TMDB API key: https://www.themoviedb.org/settings/api
3. Set the `TMDB_API_KEY` environment variable:

   PowerShell:
   ```powershell
   $Env:TMDB_API_KEY = "YOUR_API_KEY"
   ```

   To set it permanently (in a new terminal session):
   ```powershell
   setx TMDB_API_KEY "YOUR_API_KEY"
   ```

## Usage

From the project folder, run:

```powershell
cd c:\Users\USER-PC\Documents\backend-projects\TMDB-CLI-Tool

dotnet run -- --type popular

dotnet run -- --type top

dotnet run -- --type upcoming

dotnet run -- --type playing
```

Supported values for `--type`:

- `popular`
- `top`
- `upcoming`
- `playing`

## Implementation Notes

- The CLI fetches the first page of TMDB results.
- The API key is read from the `TMDB_API_KEY` environment variable.
- The application uses `HttpClient` with a timeout and handles common failures.

## Troubleshooting

- If the API key is missing, the app will report the missing `TMDB_API_KEY` environment variable.
- If the network request fails, the app will print a network error or timeout message.
- If the API response cannot be parsed, the app will print a JSON parsing error.

## URL
https://roadmap.sh/projects/tmdb-cli