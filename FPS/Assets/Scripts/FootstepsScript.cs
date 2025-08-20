using System.Collections.Generic;
using UnityEngine;

public class FootstepsScript : MonoBehaviour
{
    private List<SoundType> walkSounds;
    private List<SoundType> runSounds;

    private void Awake() {
        walkSounds = new List<SoundType>();
        walkSounds.Add(SoundType.STEP1);
        walkSounds.Add(SoundType.STEP2);
        walkSounds.Add(SoundType.STEP3);
        walkSounds.Add(SoundType.STEP4);
        runSounds = new List<SoundType>();
        runSounds.Add(SoundType.RUN1);
        runSounds.Add(SoundType.RUN2);
        runSounds.Add(SoundType.RUN3);
    }
    public void PlaySoundWalk() {
        int choice = Random.Range(0, 4);
        SoundManager.PlaySound(walkSounds[choice], 1);
    }

    public void PlaySoundRun() {
        int choice = Random.Range(0, 3);
        SoundManager.PlaySound(runSounds[choice], 1);
    }
}
