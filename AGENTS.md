# Repository Guidelines

## Project Structure & Module Organization
- `Assets/UberMaterialPropertyDrawer/Editor/` contains the main editor-only C# drawers and helpers.
- `Assets/UberMaterialPropertyDrawer/Samples/` holds sample assets for usage examples.
- `Assets/Settings/` and `ProjectSettings/` store Unity configuration.
- `Packages/` holds Unity Package Manager manifest and lock files.
- `ExportedPackages/` is used for exported `.unitypackage` artifacts.

## Build, Test, and Development Commands
- This repository does not include scripted build or test commands.
- Open the project in Unity Editor **2022.3.22f1** (from Unity Hub) for development.

## Coding Style & Naming Conventions
- Language: C# (Unity Editor extension).
- Follow standard C# conventions; no formatter or linter is enforced.
- Indentation: 4 spaces; braces on the same line as declarations.
- Naming: namespaces `PascalCase` (e.g., `ExtEditor.UberMaterialPropertyDrawer`), types/methods `PascalCase`, private fields `_camelCase`.
- Place editor-only code under an `Editor/` folder to keep it out of runtime builds.
- New comments should be written in English for consistency.

## Testing Guidelines
- No automated tests are currently included.
- Do not add tests unless explicitly requested.

## Commit & Pull Request Guidelines
- Recent commit messages are short, descriptive summaries (Japanese/English mix; no fixed prefix rules).
- No pull request requirements are currently defined.

## Package Usage
- This repository is intended to be consumed as a UPM package from a Git URL.
- Example (from README): `git@github.com:HarukaKajita/UberMaterialPropertyDrawer.git?path=Assets/UberMaterialPropertyDrawer`.
