using UnityEngine;

namespace ColorRoad.Systems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class InstantAudioSource : MonoBehaviour
    {
        private AudioSource audioSource;

        public static InstantAudioSource Create(GameObject parent, AudioClip clip)
        {
            GameObject newGameObject = new GameObject(clip.name, typeof(InstantAudioSource));
            newGameObject.transform.SetParent(parent.transform);
            InstantAudioSource component = newGameObject.GetComponent<InstantAudioSource>();
            component.audioSource.clip = clip;
            component.audioSource.Play();
            return component;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}