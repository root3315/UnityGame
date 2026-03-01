using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityGame.Addressables
{
    /// <summary>
    /// Example component for loading sprites using Addressables.
    /// Supports loading single sprites or groups by label.
    /// </summary>
    public class AddressableSpriteLoader : MonoBehaviour
    {
        [Header("Single Sprite Settings")]
        [Tooltip("The address key for the sprite in Addressables")]
        public string spriteAddress;

        [Tooltip("Target renderer to apply the sprite")]
        public SpriteRenderer targetRenderer;

        [Header("Group Loading Settings")]
        [Tooltip("Load all sprites with this label")]
        public string spriteLabel;

        [Tooltip("Cache loaded sprites for quick access")]
        public bool cacheSprites = true;

        private Dictionary<string, Sprite> _spriteCache = new();
        private List<AsyncOperationHandle> _activeHandles = new();

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(spriteAddress))
            {
                LoadSprite(spriteAddress);
            }
            else if (!string.IsNullOrEmpty(spriteLabel))
            {
                LoadSpritesByLabel(spriteLabel);
            }
        }

        /// <summary>
        /// Loads a single sprite by address key.
        /// </summary>
        public void LoadSprite(string address, System.Action<Sprite> onComplete = null)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(address);
            _activeHandles.Add(handle);

            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    if (cacheSprites)
                    {
                        _spriteCache[address] = op.Result;
                    }

                    if (targetRenderer != null)
                    {
                        targetRenderer.sprite = op.Result;
                    }

                    onComplete?.Invoke(op.Result);
                }
                else
                {
                    Debug.LogError($"Failed to load sprite: {address} - {op.OperationException}");
                }
            };
        }

        /// <summary>
        /// Loads all sprites with a specific label.
        /// </summary>
        public void LoadSpritesByLabel(string label, System.Action<List<Sprite>> onComplete = null)
        {
            var handle = Addressables.LoadAssetsAsync<Sprite>(label, null);
            _activeHandles.Add(handle);

            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var spriteList = new List<Sprite>(op.Result);

                    if (cacheSprites)
                    {
                        foreach (var sprite in op.Result)
                        {
                            if (sprite != null && !_spriteCache.ContainsKey(sprite.name))
                            {
                                _spriteCache[sprite.name] = sprite;
                            }
                        }
                    }

                    onComplete?.Invoke(spriteList);
                }
                else
                {
                    Debug.LogError($"Failed to load sprites with label: {label} - {op.OperationException}");
                }
            };
        }

        /// <summary>
        /// Gets a cached sprite by address.
        /// </summary>
        public Sprite GetCachedSprite(string address)
        {
            return _spriteCache.TryGetValue(address, out var sprite) ? sprite : null;
        }

        /// <summary>
        /// Applies a cached sprite to the target renderer.
        /// </summary>
        public bool ApplyCachedSprite(string address)
        {
            if (_spriteCache.TryGetValue(address, out var sprite) && targetRenderer != null)
            {
                targetRenderer.sprite = sprite;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Releases a specific sprite.
        /// </summary>
        public void ReleaseSprite(string address)
        {
            if (_spriteCache.TryGetValue(address, out var sprite))
            {
                Addressables.Release(sprite);
                _spriteCache.Remove(address);
            }
        }

        /// <summary>
        /// Releases all loaded sprites.
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var sprite in _spriteCache.Values)
            {
                Addressables.Release(sprite);
            }
            _spriteCache.Clear();

            foreach (var handle in _activeHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _activeHandles.Clear();
        }

        private void OnDestroy()
        {
            ReleaseAll();
        }
    }
}
