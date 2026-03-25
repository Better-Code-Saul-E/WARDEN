# Warden

Warden is a command-line file organization tool that automatically structures directories using customizable rules features a transaction-based **Undo System** to reverse operations.

![Version](https://img.shields.io/github/v/release/Better-Code-Saul-E/WARDEN)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

---

## Table of Contents
- [Download & Run](#download--run-no-net-sdk-required)
- [Usage Guide](#usage-guide)
- [Features](#features)
- [Safety & Reliability](#safety--reliability)
- [Technical Architecture](#technical-architecture)
- [Tech Stack](#tech-stack)

---

## Download & Run (No .NET SDK Required)

You do not need to install the .NET SDK to use this. You can download the standalone application for Mac (Silicon/M-Series), Windows, or Linux.

1. **[Download the latest release here](https://github.com/Better-Code-Saul-E/WARDEN/releases/latest)**
2. Open your **Terminal** and go to your downloads:

```bash
cd ~/Downloads
```

3. Make the file executable (Mac/Linux only) and run it:

```bash
chmod +x warden
./warden --help
```

> **Pro Tip:** Move the executable to your `/usr/local/bin` folder to run `warden` from any directory!

---

## Usage Guide

Warden uses intuitive verbs to manage your files. Here are the core commands:

| Command | Description |
| :--- | :--- |
| `probe [path]` | Visualize the file organization without moving files. |
| `sort [path]` | Organize files in the target directory into subfolders. **Defaults to sorting by category** (e.g., `/Images`, `/Docs`). |
| `undo` | **Reverses the last operation.** Moves files back to their original locations. |
| `undo --force` | Reverses the last operation but skips files that are missing/deleted without crashing. |
| `audit` | View the history of file movements and batch operations. |
| `help` | Show the list of available commands and flags. |

### Examples

**Sort a messy downloads folder:**

```bash
warden sort ~/Downloads
```

Oops! Didn't mean to do that? Undo it:

```bash
warden undo
```

Force an undo even if files are missing (skips errors):

```bash
warden undo --force
```

---

## Features

- **Smart Organization:** Automatically detects file types (Images, Documents, Archives, Code) and moves them into clean, labeled subdirectories.
- **Transaction-Based Undo:** Every sort operation records a unique "Batch ID". The `undo` command uses this to strictly reverse only the most recent action.
- **Audit Logging:** Maintains a local JSON ledger (`warden_log.json`) of every file moved, including timestamps and original paths.
- **Absolute Path Tracking:** Logs store absolute paths, meaning you can run the `undo` command from anywhere in your terminal, not just the target folder.

---

## Safety & Reliability

Warden is built with a "Safety First" philosophy:

- **Non-Destructive:** Warden moves files; it never deletes them.
- **Conflict Resolution:** If a file with the same name exists in the destination, Warden handles it gracefully rather than overwriting your data.
- **Crash-Proof Undo:** The Undo system performs "Pre-Flight Checks". If you manually deleted a file after sorting it, the `undo` command detects the "Ghost File," logs a warning, and skips it instead of crashing.

---

## Technical Architecture

### Warden.CLI (Main Application)

The project follows **Clean Architecture principles**, implemented as layered modules inside the main CLI project to ensure separation of concerns and testability.

-   **Presentation (Warden.CLI):** Handles user input via `Spectre.Console`. Commands (`SortCommand`, `UndoCommand`) act as controllers that delegate work to the Application layer.
-   **Application:** Contains the business orchestration.
    -   `FileOrganizerService`: Coordinates file processing.
    -   `AuditService`: Manages transaction logs for the Audit and Undo system.
-   **Domain:** Encapsulates core logic using the **Strategy Pattern**.
    -   `ISortRule`: Interface implemented by specific rules (`ExtensionSortRule`, `CategorySortRule`) to determine file organization logic dynamically.
-   **Infrastructure:** Implements system-level operations.
    -   `PhysicalFileSystem`: Wraps `System.IO` to allow for safe disk operations and mockability during testing.
-   **Dependency Injection:** Uses `Microsoft.Extensions.DependencyInjection` bridged with `Spectre.Console` for modular service management.

---

### Warden.CLI.Tests (Test Project)

A separate test project containing:

- **Unit Tests:** Verify business logic in isolation using mocks.
- **Integration Tests:** Validate real-world behavior using temporary test directories.

---

## Tech Stack

- **Language:** C# (.NET 8.0)
- **CLI Framework:** System.CommandLine
- **UI/UX:** Spectre.Console (Rich text, Tables)
- **Dependency Injection:** Microsoft.Extensions.DependencyInjection
- **Testing:** xUnit, Moq
