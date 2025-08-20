using UnityEngine;

public enum SoundType
{ 
    RELOAD_AR,
    RELOAD_SHOTGUN,
    LAND,
    GRUNT,
    HEAL,
    DOOR,
    ENEMY_DEATH,
    STEP1,
    STEP2,
    STEP3,
    STEP4,
    RUN1,
    RUN2,
    RUN3,
    PLAYER_DEATH_GRUNT,
    PLAYER_DEATH_EFFECT
}
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1) {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
