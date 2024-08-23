using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SunkenRuins {
    public class SFXManager : MonoBehaviour {
        public static SFXManager instance { get; private set;}
        public AudioClip[] audioClips;
        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            }
            else {
                instance = this;
            }
        }
        public void PlaySFX(int index) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[index];
            audioSource.volume = 0.5f;
            audioSource.Play();

            StartCoroutine(DestroyAfterPlaying(audioSource));
        }

        private IEnumerator DestroyAfterPlaying(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioSource.clip.length);

            Destroy(audioSource);
        }
    }
}
