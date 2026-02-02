# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

StoryWriter is a .NET 8.0 multi-platform book/story management application with a three-tier architecture:

- **StoryWriter/** - Shared class library containing domain models and MongoDB data access
- **StoryWriterClient/** - .NET MAUI cross-platform client (Windows, macOS, Android, iOS)
- **WebApi/** - ASP.NET Core REST API backend

## Build and Run Commands

```bash
# Restore dependencies
dotnet restore

# Build entire solution
dotnet build

# Run the Web API (requires MongoDB on localhost:27017)
dotnet run --project WebApi/

# Run MAUI client (Windows)
dotnet run --project StoryWriterClient/ -f net8.0-windows10.0.19041.0

# Run MAUI client (macOS)
dotnet run --project StoryWriterClient/ -f net8.0-maccatalyst

# Run MAUI client (Android)
dotnet run --project StoryWriterClient/ -f net8.0-android
```

## Architecture

### Data Flow
```
MAUI Client (HTTP) -> WebApi (REST) -> BookService -> DataBaseService -> MongoDB
```

### Key Components

**Domain Models** (`StoryWriter/Classes/`):
- `Book` - Main entity with Title, Author, and optional Chapters list
- `Chapter` - Book chapter with Name, Number, and Content
- `Character` - Stub class (not implemented)

**Data Layer** (`StoryWriter/BDD/`):
- `BddSettings` - Static configuration for MongoDB connection string and database name
- `DataBaseService` - MongoDB client initialization and collection access
- `BookService` - CRUD operations for Book entities (all synchronous)

**API Layer** (`WebApi/Controllers/`):
- `BookController` - REST endpoints at `/Book` for all book operations

**Client Views** (`StoryWriterClient/Vues/`):
- `MainPage` - Book list display with navigation
- `CreateBook` - New book form with Title/Author fields
- `UpdateBook` - Rich text editor using Quill.js via WebView

### Configuration

- MongoDB connection: Hardcoded in `WebApi/Program.cs` as `mongodb://localhost:27017`
- API endpoint: Hardcoded in client views as `https://localhost:7056`
- Swagger UI available at `/swagger` in Development mode

## Current Limitations

- No authentication/authorization implemented
- No unit tests
- Empty exception handlers throughout codebase
- Character class is a stub
- Chapter.Content is stored as `int` (likely should be `string`)
- All data services are static with synchronous operations
- Client generates MongoDB ObjectId (should be server-side)

## Dependencies

- MongoDB.Driver 3.2.1 (data access)
- Newtonsoft.Json 13.0.3 (JSON serialization in client)
- Swashbuckle.AspNetCore 6.6.2 (Swagger/OpenAPI)
- Quill.js 1.3.6 (rich text editor loaded via CDN)
