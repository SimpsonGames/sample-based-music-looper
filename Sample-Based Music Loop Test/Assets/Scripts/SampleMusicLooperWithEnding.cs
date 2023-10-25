using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SampleMusicLooperWithEnding : SampleMusicLooper
{
    [Space]
    [SerializeField] private AudioClip endingClip;

    private void Update()
    {
        if (hasEnded) return;
        
        if (!Input.GetKeyDown(KeyCode.P)) return;
        
        hasEnded = true;
        audioSource.clip = endingClip;
        audioSource.Play();
    }
}
