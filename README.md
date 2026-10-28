# mr001

A Unity project using Universal Render Pipeline (URP), XR Interaction Toolkit, and OpenXR.

## Requirements

- **Unity Hub**
- **Unity Editor 6000.6.7f1**, matching `ProjectSettings/ProjectVersion.txt`
- **Git** or GitHub Desktop
- Internet access to download packages on first launch
- Build Support modules for your target platform, such as Android Build Support for Android builds

Coordinate with collaborators before upgrading Unity or packages. Upgrades may change scenes, assets, and project settings.

## Getting Started

### 1. Clone the Repository

On this repository's GitHub page, select **Code** and copy the HTTPS URL. Replace `<repository-url>` below with that URL before running the command in PowerShell:

```powershell
git clone <repository-url>
```

Alternatively, use **Open with GitHub Desktop**. Private repositories require access permission.

### 2. Open in Unity Hub

1. Open Unity Hub.
2. Select **Projects → Add → Add project from disk**.
3. Choose the cloned folder containing `Assets/`, `Packages/`, and `ProjectSettings/`.
4. Open it with **Unity Editor 6000.6.7f1**.
5. Wait for package downloads, asset imports, and script compilation to finish.

Unity generates `Library/`, `Temp/`, and other local files automatically. The first launch may take longer than subsequent launches. You do not need to copy caches from another computer.

### 3. Open a Scene

Available project scenes:

- `Assets/Scenes/Main VR Scene.unity`
- `Assets/Scenes/SampleScene.unity`

Double-click a scene in the Project window. Check the Console for errors before entering Play mode.

> Installed XR packages do not confirm that a headset or simulator is configured. Check XR Plug-in Management, OpenXR settings, and the target device separately. Entering Play mode alone does not verify operation on a physical headset.

## Main Packages

Versions below reflect `Packages/manifest.json` when this README was written.

| Package | Version |
| --- | --- |
| Universal Render Pipeline | `17.6.0` |
| Input System | `1.20.1` |
| XR Interaction Toolkit | `3.6.1` |
| XR Plug-in Management | `4.7.0` |
| OpenXR Plugin | `1.18.0` |
| Unity Pipeline | `0.9.1-exp.1` |

Unity Package Manager uses `manifest.json` and `packages-lock.json` to manage dependencies. Keep both in Git rather than sharing another computer's `Library/` folder.

## Project Structure

| Path | Version control guidance |
| --- | --- |
| `Assets/` | Keep scenes, prefabs, scripts, materials, models, textures, and their `.meta` files |
| `Packages/` | Keep package declarations and `packages-lock.json` |
| `ProjectSettings/` | Keep shared settings and the Editor version |
| `.gitignore` | Keep rules excluding generated and machine-specific files |
| `README.md` | Keep setup and collaboration instructions |
| `Output/` | Not currently ignored; review its contents before committing |

### Files Excluded from Git

The `.gitignore` excludes:

- Unity caches: `Library/`, `Temp/`, and `Obj/`
- Logs and local settings: `Logs/` and `UserSettings/`
- Build output: `Build/` and `Builds/`
- Root-level memory captures and recordings
- Generated IDE files, including root-level `.vs/`, `.idea/`, `*.csproj`, `*.sln`, and `*.slnx`
- `.env` and `.env.*`, except templates such as `.env.example`
- Android signing keys: `*.keystore` and `*.jks`

**Do not add blanket ignore rules for `.meta` files, models, textures, or plugin DLLs.** These may be required for the project to work. For large assets, consider Git LFS and agree on its configuration before adding files. If Git LFS is adopted, collaborators must install it and download the LFS assets too.

> `.gitignore` does not delete local files or automatically stop tracking files already committed. Review changes before every commit.

## Collaboration Workflow

Run these commands from the project root.

### Before Starting Work

```powershell
git status
# Pull the latest changes when your working tree is clean.
git pull --ff-only
```

If you have unfinished changes, commit or otherwise preserve them first. Do not use reset or clean commands to bypass the issue without reviewing what would be lost.

### After Making Changes

1. Save scenes and assets in Unity.
2. Check the Console and test the affected functionality.
3. Review the changes before staging them.

```powershell
git status
git diff
# Stage specific files, or use git add . after reviewing all changes.
git add .
git diff --cached --stat
git diff --cached
git commit -m "Update scene"
git push
```

The push command assumes your branch already tracks a remote branch. Follow the team's branch and pull request conventions.

### Recommended Practices

- Always commit `.meta` files alongside their assets.
- Move and rename assets through Unity's Project window so their `.meta` files move with them.
- Avoid editing the same scene or prefab simultaneously. Divide work or use separate prefabs where practical.
- Use **Force Text** asset serialization and **Visible Meta Files** in the relevant Project Settings sections to help with reviewing and merging changes. This guide does not verify or change those settings.
- Never commit tokens, passwords, secrets, or signing keys. Back up signing keys securely outside Git.
- For packages referencing files outside this repository or private Git repositories, provide collaborators with the required files and permissions.

## Troubleshooting

### Missing Library Folder

This is expected after cloning. Open the project through Unity Hub and allow Unity to regenerate it. Do not copy `Library/` from another computer.

### Package Download Errors

Check your connection and the errors in Package Manager or the Console. Verify permissions for packages requiring an account or special access. Avoid changing package versions at random.

### Missing Scripts or References

Confirm that the Editor version matches, scripts compile, packages are installed, and required assets and `.meta` files were committed. Do not delete `.meta` files as a quick fix; doing so can break existing references.

### git pull --ff-only Fails

Check `git status`. If your branch and the remote have diverged, agree on a merge or rebase approach with the team. Do not force-push or reset other people's work without reviewing the consequences.

## Verification Scope

This README is based on the project's Editor version, package manifest, and available scene files. It is not a test report for a fresh clone, Play mode, a build, or operation on a physical headset.

