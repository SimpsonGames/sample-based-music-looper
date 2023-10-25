using System.Collections.Generic;
using UnityEngine;
using NVorbis;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SampleMusicLooper : MonoBehaviour
{
    [HideInInspector] public AudioSource audioSource;

    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool playOnStart;

    [HideInInspector] public bool hasEnded;

    //The sample the audioSource will be set to when it loops
    private int _startSample;
    //The sample that, when reached, will loop the song
    private int _loopSample;
    
    //The path to the basic project
    private const string PROJECT_PATH = ".\\Assets";
    //The path that the looping file is saved
    //Filename added in Start() method
    private const string LOOPABLE_SONG_PATH = "\\"; //Place directory to audio folder here!

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        /*https://github.com/NVorbis/NVorbis - NVorbis Library by NVorbis*/
        using VorbisReader f = new(PROJECT_PATH + LOOPABLE_SONG_PATH + $"{musicClip.name}.ogg");
        foreach (KeyValuePair<string, IReadOnlyList<string>> c in f.Tags.All)
        {
            Debug.Log(c);

            //Get the LOOPSTART and LOOPLENGTH values from the .ogg file
            switch (c.Key)
            {
                case "LOOPSTART":
                    _startSample = int.Parse(c.Value[0]);
                    break;
                case "LOOPLENGTH":
                    _loopSample = int.Parse(c.Value[0]);
                    break;
            }
        }

        //Add startSample to loopSample because it gets deducted at some point in the process
        _loopSample += _startSample;
        
        //Set music clip and play if playOnStart is true
        audioSource.clip = musicClip;
        if (playOnStart) audioSource.Play();
        
        //TODO - Remove at some point
        Debug.Log($"Playing {musicClip} at {musicClip.samples}");
    }

    private void Update()
    {
        //Check if song has been ended for whatever reason
        if (hasEnded) return;
        
        //If the loopSample variable is equal to 0, the control over looping/playing will 
        if (_loopSample == 0)
            return;

        //If the current sample number is less than the loopSample value, do not continue
        if (audioSource.timeSamples < _loopSample) return;
        
        Debug.Log($"Looped song with filename: {musicClip.name}");
        audioSource.timeSamples = _startSample;
    }
    
    public void UpdateDebugText(TMP_Text displayText)
    {
        int sample = audioSource.timeSamples;
        float progress = ((float)sample / _loopSample) * 100f;

        displayText.text = $"Current Song: {musicClip.name}\n \n" +
                           $"Current Sample: {sample}\n" +
                           $"Start Sample: {_startSample}\n" +
                           $"Loop Sample: {_loopSample}\n" +
                           $"Sample Progress: {sample}/{_loopSample} ({progress:F1}%)";
    }
}
