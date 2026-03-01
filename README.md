# UnityGame

A Unity project utilizing the Addressable Asset System for optimized asset management.

## Features

- **Addressable Asset System**: On-demand loading for music and sprite resources
- **Memory Efficiency**: Automatic asset unloading when not in use
- **Build Optimization**: Bundled assets for reduced build sizes
- **Content Updates**: Update content without rebuilding the entire game

## Project Structure

```
UnityGame/
├── Игра/
│   ├── ADDRESSABLES_SETUP.md       # Detailed setup guide
│   ├── AddressableAssetsData/      # Addressables configuration
│   │   ├── AddressableAssetSettings.asset
│   │   ├── AssetGroups/            # Asset group definitions
│   │   ├── AssetGroupTemplates/
│   │   └── DataBuilders/
│   ├── Assets/
│   │   └── Scripts/
│   │       └── Addressables/       # Loading scripts
│   ├── Packages/
│   │   └── manifest.json           # Package dependencies
│   ├── музыка/                     # Music files (.mp3)
│   └── спрайты/                    # Sprite files
├── .gitignore
└── README.md
```

## Quick Start

### 1. Install Dependencies

The project requires the Addressables package. Open `Игра/Packages/manifest.json` to verify:

```json
{
  "dependencies": {
    "com.unity.addressables": "1.21.21"
  }
}
```

### 2. Initialize Addressables

1. Open the Unity project
2. Navigate to **Window > Asset Management > Addressables > Groups**
3. Click **Create Addressables Settings** if prompted

### 3. Configure Assets

1. Select assets in `музыка/` or `спрайты/`
2. In the Inspector, check **Addressable Assets**
3. Assign unique addresses (e.g., `music/desert_theme`, `sprites/hero`)
4. Add appropriate labels for grouping

## Usage

### Loading Music

```csharp
using UnityGame.Addressables;

var manager = AddressableAssetManager.Instance;
manager.LoadAudioAsync("music/desert_theme", clip => {
    // Play the audio clip
});
```

### Loading Sprites

```csharp
// Load single sprite
manager.LoadSpriteAsync("sprites/characters/hero", sprite => {
    // Use the sprite
});

// Load multiple sprites by label
manager.LoadSpritesByLabelAsync("characters", sprites => {
    // Process all character sprites
});
```

### Releasing Assets

```csharp
// Release specific asset
manager.ReleaseAudio("music/desert_theme");

// Release all assets
manager.ReleaseAll();
```

## Documentation

For detailed setup instructions and best practices, see:
- [ADDRESSABLES_SETUP.md](Игра/ADDRESSABLES_SETUP.md) - Complete configuration guide

## Best Practices

1. **Naming Convention**: Use `category/name` format for addresses
2. **Labels**: Group assets with labels (music, sprites, characters, environments)
3. **Memory Management**: Always release assets when done
4. **Async Loading**: Use async methods to prevent frame drops

## Requirements

- Unity 2019.4 or later
- Addressables package 1.21.21
