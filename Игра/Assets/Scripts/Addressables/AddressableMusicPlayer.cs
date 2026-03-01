using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityGame.Addressables
{
    /// <summary>
    /// Example component for loading and playing music using Addressables.
    /// Attach to a GameObject with an AudioSource component.
    /// </summary>
    public class AddressableMusicPlayer : MonoBehaviour
    {
        [Header("Addressable Settings")]
        [Tooltip("The address key for the audio clip in Addressables")]
        public string musicAddress;

        [Tooltip("Load on start automatically")]
        public bool loadOnStart = true;

        [Header("Audio Source")]
        public AudioSource audioSource;

        private AudioClip _currentClip;
        private AsyncOperationHandle<AudioClip> _loadHandle;

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        private void Start()
        {
            if (loadOnStart && !string.IsNullOrEmpty(musicAddress))
            {
                LoadMusic(musicAddress);
            }
        }

        /// <summary>
        /// Loads music by address key.
        /// </summary>
        public void LoadMusic(string address)
        {
            // Release previous clip if exists
            if (_currentClip != null && _loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
                _currentClip = null;
            }

            musicAddress = address;
            _loadHandle = Addressables.LoadAssetAsync<AudioClip>(address);
            _loadHandle.Completed += OnMusicLoaded;
        }

        /// <summary>
        /// Loads music by label (loads random clip from label group).
        /// </summary>
        public void LoadMusicByLabel(string label)
        {
            var handle = Addressables.LoadAssetsAsync<AudioClip>(label, null);
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)
                {
                    int randomIndex = Random.Range(0, op.Result.Count);
                    SetClip(op.Result[randomIndex]);
                }
                else
                {
                    Debug.LogError($"Failed to load music with label: {label}");
                }
            };
        }

        private void OnMusicLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                SetClip(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to load music: {musicAddress} - {handle.OperationException}");
            }
        }

        private void SetClip(AudioClip clip)
        {
            _currentClip = clip;
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }

        /// <summary>
        /// Plays the loaded music.
        /// </summary>
        public void Play()
        {
            if (_currentClip != null)
            {
                audioSource.Play();
            }
        }

        /// <summary>
        /// Pauses the music.
        /// </summary>
        public void Pause()
        {
            audioSource.Pause();
        }

        /// <summary>
        /// Stops the music and releases the asset.
        /// </summary>
        public void StopAndRelease()
        {
            audioSource.Stop();
            if (_currentClip != null && _loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
                _currentClip = null;
            }
        }

        private void OnDestroy()
        {
            if (_currentClip != null && _loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
            }
        }
    }
}
