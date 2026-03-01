# Unity Addressable Asset System Setup

This guide explains how to use Unity's Addressable Asset System for your music and sprite resources to optimize asset loading and reduce build sizes.

## Overview

The Addressable Asset System provides:
- **On-demand loading**: Load assets only when needed
- **Memory efficiency**: Unload assets when not in use
- **Build size optimization**: Bundle assets separately
- **Remote content**: Download assets from remote servers (optional)
- **Content updates**: Update content without rebuilding the entire game

## Project Structure

```
Игра/
├── AddressableAssetsData/          # Addressables configuration
│   ├── AssetGroups/                # Asset group definitions
│   │   ├── Schemas/                # Group schema files
│   │   ├── Music.asset             # Music asset group
│   │   └── Sprites.asset           # Sprites asset group
│   ├── AssetGroupTemplates/        # Group templates
│   └── AddressableAssetSettings.asset
├── Assets/
│   └── Scripts/
│       └── Addressables/           # Loading scripts
│           ├── AddressableAssetManager.cs
│           ├── AddressableMusicPlayer.cs
│           ├── AddressableSpriteLoader.cs
│           └── AddressableAtlasLoader.cs
├── Packages/
│   └── manifest.json               # Package dependencies
├── музыка/                         # Music files
└── спрайты/                        # Sprite files
```

## Installation

### 1. Install Addressables Package

The `Packages/manifest.json` already includes the Addressables package:

```json
{
  "dependencies": {
    "com.unity.addressables": "1.21.21"
  }
}
```

### 2. Open Addressables Window

In Unity Editor:
1. Go to **Window > Asset Management > Addressables > Groups**
2. The first time you open it, Unity will prompt to initialize Addressables
3. Click **Create Addressables Settings** if prompted

## Configuring Assets

### Step 1: Mark Assets as Addressable

#### For Music Files:
1. Navigate to `Игра/музыка/`
2. Select your `.mp3` files
3. In the Inspector, check **Addressable Assets**
4. Set the **Address** to a unique identifier (e.g., `music/desert_theme`)
5. Add the **music** label

#### For Sprite Files:
1. Navigate to `Игра/спрайты/`
2. Select your sprite files
3. In the Inspector, check **Addressable Assets**
4. Set the **Address** (e.g., `sprites/characters/hero`)
5. Add appropriate labels: **sprites**, **characters**, **environments**, or **dungeons**

### Step 2: Organize into Groups

The configuration already includes two groups:
- **Music**: For all audio files
- **Sprites**: For all sprite files

To assign assets to groups:
1. Open **Window > Asset Management > Addressables > Groups**
2. Drag assets from the Project window into the appropriate group

### Step 3: Configure Build Settings

In the Addressables Groups window:
1. Click the **Play Mode Script** dropdown
2. Select **Use Existing Build** for testing with built bundles
3. Select **Fast Mode** for quick iteration during development

## Using the Loading Scripts

### AddressableAssetManager

Central manager for loading assets:

```csharp
using UnityGame.Addressables;

// Get the instance
var manager = AddressableAssetManager.Instance;

// Load music
manager.LoadAudioAsync("music/desert_theme", clip => {
    Debug.Log("Music loaded!");
});

// Load sprite
manager.LoadSpriteAsync("sprites/characters/hero", sprite => {
    Debug.Log("Sprite loaded!");
});

// Load all sprites by label
manager.LoadSpritesByLabelAsync("characters", sprites => {
    Debug.Log($"Loaded {sprites.Count} character sprites");
});

// Release assets when done
manager.ReleaseAudio("music/desert_theme");
manager.ReleaseSprite("sprites/characters/hero");
```

### AddressableMusicPlayer

Component for playing music:

```csharp
// Attach to a GameObject with AudioSource
var player = gameObject.AddComponent<AddressableMusicPlayer>();
player.musicAddress = "music/desert_theme";
player.loadOnStart = true;

// Or load dynamically
player.LoadMusic("music/village_theme");
player.LoadMusicByLabel("music");  // Random from label group
```

### AddressableSpriteLoader

Component for loading sprites:

```csharp
// Attach to a GameObject with SpriteRenderer
var loader = gameObject.AddComponent<AddressableSpriteLoader>();
loader.spriteAddress = "sprites/characters/hero";
loader.targetRenderer = GetComponent<SpriteRenderer>();

// Or load by label
loader.LoadSpritesByLabel("dungeons");

// Get cached sprite
var cached = loader.GetCachedSprite("sprites/characters/hero");
```

### AddressableAtlasLoader

For Sprite Atlas assets:

```csharp
var atlasLoader = gameObject.AddComponent<AddressableAtlasLoader>();
atlasLoader.atlasAddress = "atlases/character_atlas";
atlasLoader.LoadAtlas("atlases/character_atlas");

// Get sprite from atlas
var sprite = atlasLoader.GetSpriteFromAtlas("hero_idle");
```

## Best Practices

### 1. Address Naming Convention
Use consistent naming:
- `music/{track_name}` for music
- `sprites/{category}/{asset_name}` for sprites
- `atlases/{atlas_name}` for sprite atlases

### 2. Label Usage
Labels enable loading groups of assets:
- **music**: All music tracks
- **sprites**: All sprites
- **characters**: Character sprites only
- **environments**: Environment tiles
- **dungeons**: Dungeon-specific assets

### 3. Memory Management
Always release assets when done:
```csharp
// In OnDestroy or when scene unloads
manager.ReleaseAll();
```

### 4. Async Loading
Use async loading to prevent frame drops:
```csharp
// Show loading screen while assets load
var handle = manager.LoadSpriteAsync(address);
loadingScreen.Show();
handle.Completed += _ => loadingScreen.Hide();
```

### 5. Build Profiles

Create different profiles for different platforms:

1. **Window > Asset Management > Addressables > Groups**
2. Click the **Profile** dropdown
3. Select **Create Profile**
4. Configure build paths for each platform

## Building Addressables

### Local Build (Testing)
1. **Window > Asset Management > Addressables > Groups**
2. Click **Build > New Build > Default Build Script**
3. Assets are built to `StreamingAssets`

### Remote Build (Production)
1. Create a remote profile with CDN/server paths
2. Build with **Build > New Build > Default Build Script**
3. Upload bundle files to your server

## Troubleshooting

### Assets Not Loading
- Verify assets are marked as Addressable
- Check the address key is correct
- Ensure the Addressables build has been run

### High Memory Usage
- Release assets when not needed
- Use `AddressableAssetManager.ReleaseAll()` on scene unload
- Check for duplicate loading

### Build Errors
- Open Addressables Groups window
- Click **Tools > Analyze** to find issues
- Fix any reported problems

## Additional Resources

- [Unity Addressables Documentation](https://docs.unity3d.com/Packages/com.unity.addressables@latest)
- [Addressables Best Practices](https://blog.unity.com/technology/addressables-best-practices)
- [Memory Management Guide](https://docs.unity3d.com/Packages/com.unity.addressables@latest?subfolder=/manual/MemoryManagement.html)
