using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityGame.Addressables
{
    /// <summary>
    /// Centralized manager for loading addressable assets (music and sprites).
    /// Provides async loading with caching and error handling.
    /// </summary>
    public class AddressableAssetManager : MonoBehaviour
    {
        private static AddressableAssetManager _instance;
        public static AddressableAssetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AddressableAssetManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("AddressableAssetManager");
                        _instance = go.AddComponent<AddressableAssetManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private readonly Dictionary<string, AudioHandle> _loadedAudio = new();
        private readonly Dictionary<string, SpriteHandle> _loadedSprites = new();

        /// <summary>
        /// Preloads all assets in a group by label.
        /// </summary>
        public AsyncOperationHandle<IList<IResourceLocation>> PreloadGroupAsync(string label)
        {
            return Addressables.LoadResourceLocationsAsync(label, typeof(object));
        }

        /// <summary>
        /// Loads an audio clip asynchronously.
        /// </summary>
        public AsyncOperationHandle<AudioClip> LoadAudioAsync(string address, Action<AudioClip> onComplete = null)
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(address);
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var audioHandle = new AudioHandle { AudioClip = op.Result, Address = address };
                    _loadedAudio[address] = audioHandle;
                    onComplete?.Invoke(op.Result);
                }
                else
                {
                    Debug.LogError($"Failed to load audio: {address} - {op.OperationException}");
                }
            };
            return handle;
        }

        /// <summary>
        /// Loads a sprite asynchronously.
        /// </summary>
        public AsyncOperationHandle<Sprite> LoadSpriteAsync(string address, Action<Sprite> onComplete = null)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(address);
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var spriteHandle = new SpriteHandle { Sprite = op.Result, Address = address };
                    _loadedSprites[address] = spriteHandle;
                    onComplete?.Invoke(op.Result);
                }
                else
                {
                    Debug.LogError($"Failed to load sprite: {address} - {op.OperationException}");
                }
            };
            return handle;
        }

        /// <summary>
        /// Loads all sprites in a group by label.
        /// </summary>
        public AsyncOperationHandle<IList<Sprite>> LoadSpritesByLabelAsync(string label, Action<IList<Sprite>> onComplete = null)
        {
            var handle = Addressables.LoadAssetsAsync<Sprite>(label, null);
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    foreach (var sprite in op.Result)
                    {
                        if (sprite != null)
                        {
                            var spriteHandle = new SpriteHandle { Sprite = sprite, Address = sprite.name };
                            _loadedSprites[sprite.name] = spriteHandle;
                        }
                    }
                    onComplete?.Invoke(op.Result);
                }
                else
                {
                    Debug.LogError($"Failed to load sprites with label {label}: {op.OperationException}");
                }
            };
            return handle;
        }

        /// <summary>
        /// Releases a specific audio clip.
        /// </summary>
        public void ReleaseAudio(string address)
        {
            if (_loadedAudio.TryGetValue(address, out var handle))
            {
                Addressables.Release(handle.AudioClip);
                _loadedAudio.Remove(address);
            }
        }

        /// <summary>
        /// Releases a specific sprite.
        /// </summary>
        public void ReleaseSprite(string address)
        {
            if (_loadedSprites.TryGetValue(address, out var handle))
            {
                Addressables.Release(handle.Sprite);
                _loadedSprites.Remove(address);
            }
        }

        /// <summary>
        /// Releases all loaded assets.
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var handle in _loadedAudio.Values)
            {
                Addressables.Release(handle.AudioClip);
            }
            _loadedAudio.Clear();

            foreach (var handle in _loadedSprites.Values)
            {
                Addressables.Release(handle.Sprite);
            }
            _loadedSprites.Clear();
        }

        /// <summary>
        /// Initializes the Addressables system.
        /// </summary>
        public AsyncOperationHandle<InitializationOperation> InitializeAsync()
        {
            return Addressables.InitializeAsync();
        }

        private void OnDestroy()
        {
            ReleaseAll();
        }
    }

    [Serializable]
    public struct AudioHandle
    {
        public AudioClip AudioClip;
        public string Address;
    }

    [Serializable]
    public struct SpriteHandle
    {
        public Sprite Sprite;
        public string Address;
    }
}
