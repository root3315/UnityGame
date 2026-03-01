using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace UnityGame.Addressables
{
    /// <summary>
    /// Loads Sprite Atlases using Addressables for optimized sprite batching.
    /// </summary>
    public class AddressableAtlasLoader : MonoBehaviour
    {
        [Header("Atlas Settings")]
        [Tooltip("Address key for the Sprite Atlas")]
        public string atlasAddress;

        [Tooltip("Automatically bind sprites on load")]
        public bool autoBind = true;

        private SpriteAtlas _currentAtlas;
        private AsyncOperationHandle<SpriteAtlas> _loadHandle;

        /// <summary>
        /// Loads a Sprite Atlas by address.
        /// </summary>
        public void LoadAtlas(string address)
        {
            // Release previous atlas if exists
            if (_currentAtlas != null && _loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
                _currentAtlas = null;
            }

            atlasAddress = address;
            _loadHandle = Addressables.LoadAssetAsync<SpriteAtlas>(address);
            _loadHandle.Completed += OnAtlasLoaded;
        }

        private void OnAtlasLoaded(AsyncOperationHandle<SpriteAtlas> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _currentAtlas = handle.Result;

                if (autoBind)
                {
                    _currentAtlas.BindAllSprites();
                }

                Debug.Log($"Sprite Atlas loaded: {atlasAddress}");
            }
            else
            {
                Debug.LogError($"Failed to load sprite atlas: {atlasAddress} - {handle.OperationException}");
            }
        }

        /// <summary>
        /// Gets a sprite from the loaded atlas by name.
        /// </summary>
        public Sprite GetSpriteFromAtlas(string spriteName)
        {
            if (_currentAtlas != null)
            {
                return _currentAtlas.GetSprite(spriteName);
            }
            return null;
        }

        /// <summary>
        /// Gets the currently loaded atlas.
        /// </summary>
        public SpriteAtlas GetCurrentAtlas()
        {
            return _currentAtlas;
        }

        /// <summary>
        /// Releases the loaded atlas.
        /// </summary>
        public void ReleaseAtlas()
        {
            if (_currentAtlas != null && _loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
                _currentAtlas = null;
            }
        }

        private void OnDestroy()
        {
            ReleaseAtlas();
        }
    }
}
