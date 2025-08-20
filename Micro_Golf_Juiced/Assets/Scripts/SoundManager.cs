using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    public static SoundManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public void PlaySoundFX(AudioClip clip, Transform spawn, float volume) {
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float cliplength = audioSource.clip.length;
        Destroy(audioSource.gameObject, cliplength);
    }

    public void PlayRandomSoundFX(AudioClip[] clip, Transform spawn, float volume) {
        int rand = Random.Range(0, clip.Length);
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
        audioSource.clip = clip[rand];
        audioSource.volume = volume;
        audioSource.Play();
        float cliplength = audioSource.clip.length;
        Destroy(audioSource.gameObject, cliplength);
    }
}
