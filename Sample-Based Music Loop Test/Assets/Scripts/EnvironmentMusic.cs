using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class EnvironmentMusic : MonoBehaviour
{
    private AudioSource _audioSource;
    
    //Daytime Music
    [SerializeField] private List<MusicPart> dayParts;

    //Part 0
    private readonly MinMax _part0Volume = new(0.5f, 1f);
    private readonly MinMax _part0Pan = new(-0.82f, 0.82f);

    //Part 1 
    private readonly MinMax _part1Volume = new(0.5f, 1f);
    private readonly MinMax _part1Pan = new(-0.4f, 0.4f);

    //Wait Count Rates (i.e. how long between each clip should we wait)
    private readonly MinMax _part0SegmentInterval = new(1f, 4f);
    private readonly MinMax _part1SegmentAInterval = new(1f, 4f);
    private readonly MinMax _part1SegmentBInterval = new(1f, 5f);
    private readonly MinMax _part1SegmentCInterval = new(1f, 5f);

    //Separate lists to divide up the parts
    private List<MusicPart> _part0Parts;
    private List<MusicPart> _part1SegmentAParts;
    private List<MusicPart> _part1SegmentBParts;
    private List<MusicPart> _part1SegmentCParts;

    //Timer for Wait Count Rates
    private float _currentWaitCount;
    private float _waitTimer;

    private MusicPart.Part _currentPart;
    private MusicPart.Segment _currentSegment;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        //Initialised here otherwise NullReferenceException thrown later for some reason idk why
        _part0Parts = new List<MusicPart>();
        _part1SegmentAParts = new List<MusicPart>();
        _part1SegmentBParts = new List<MusicPart>();
        _part1SegmentCParts = new List<MusicPart>();
    }

    private void Start()
    {
        InitPartLists();

        PlayNextMusicPart(MusicPart.Part.Part0, MusicPart.Segment.A);
    }

    private void Update()
    {
        // Do nothing if the current clip is still playing
        // Avoids overlap which causes cutouts
        if (_audioSource.isPlaying)
            return;
        
        // 1f * acts as a failsafe
        _waitTimer += 1f * Time.deltaTime;
        if (!(_waitTimer >= _currentWaitCount)) return; // Do nothing until timer has elapsed
        
        // If the current part is 0, set to 1, and vise versa
        MusicPart.Part updatedPart = _currentPart == MusicPart.Part.Part0 ? MusicPart.Part.Part1 : MusicPart.Part.Part0;

        // Generate a random segment to play next
        // 0 = A, 1 = B, 2 = C
        int segmentChoice = Random.Range(0, 3);
        MusicPart.Segment updatedSegment = segmentChoice switch
        {
            0 => MusicPart.Segment.A,
            1 => MusicPart.Segment.B,
            2 => MusicPart.Segment.C,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        //Pass new part and segment
        PlayNextMusicPart(updatedPart, updatedSegment);
        // Reset timer
        _waitTimer = 0; 
    }

    /// <summary>
    /// Initialise the part lists with each music clip depending on the part and segment
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void InitPartLists()
    {
        foreach (MusicPart part in dayParts)
        {
            if (part.musicPart == MusicPart.Part.Part0)
            {
                _part0Parts.Add(part);
                continue;
            }

            switch (part.musicSegment)
            {
                case MusicPart.Segment.A:
                    _part1SegmentAParts.Add(part);
                    break;
                case MusicPart.Segment.B:
                    _part1SegmentBParts.Add(part);
                    break;
                case MusicPart.Segment.C:
                    _part1SegmentCParts.Add(part);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// Sets and plays the next music clip based on passed part and segment
    /// </summary>
    /// <param name="newPart">0 - Will play any of the part 0 clips, 1 - Will play clip dependant on segment</param>
    /// <param name="newSegment">Is not considered if part == 0</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void PlayNextMusicPart(MusicPart.Part newPart, MusicPart.Segment newSegment)
    {
        _currentPart = newPart;
        _currentSegment = newSegment;

        if (_currentPart == MusicPart.Part.Part0)
        {
            _audioSource.clip = _part0Parts[Random.Range(0, _part0Parts.Count)].musicClip;
            _audioSource.volume = Random.Range(_part0Volume.min, _part0Volume.max);
            _audioSource.panStereo = Random.Range(_part0Pan.min, _part0Pan.max);
            
            _currentWaitCount = Random.Range(_part0SegmentInterval.min, _part0SegmentInterval.max);
        }
        else
        {
            _audioSource.clip = _currentSegment switch
            {
                MusicPart.Segment.A => _part1SegmentAParts[Random.Range(0, _part1SegmentAParts.Count)].musicClip,
                MusicPart.Segment.B => _part1SegmentBParts[Random.Range(0, _part1SegmentBParts.Count)].musicClip,
                MusicPart.Segment.C => _part1SegmentCParts[Random.Range(0, _part1SegmentCParts.Count)].musicClip,
                _ => throw new ArgumentOutOfRangeException(nameof(newSegment), newSegment, null)
            };

            _audioSource.volume = Random.Range(_part1Volume.min, _part1Volume.max);
            _audioSource.panStereo = Random.Range(_part1Pan.min, _part1Pan.max);

            _currentWaitCount = _currentSegment switch
            {
                MusicPart.Segment.A => Random.Range(_part1SegmentAInterval.min, _part1SegmentAInterval.max),
                MusicPart.Segment.B => Random.Range(_part1SegmentBInterval.min, _part1SegmentBInterval.max),
                MusicPart.Segment.C => Random.Range(_part1SegmentCInterval.min, _part1SegmentCInterval.max),
                _ => throw new ArgumentOutOfRangeException(nameof(_currentSegment), _currentSegment, null)
            };
        }
        
        _audioSource.Play();
    }
}

/// <summary>
/// Class that contains a minimum and maximum value for a range
/// </summary>
[Serializable]
public class MinMax
{
    public float min;
    public float max;

    public MinMax(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

/// <summary>
/// Class that stores a music clip, along with the part and segment associated
/// </summary>
[Serializable]
public class MusicPart
{
    public AudioClip musicClip;

    public enum Part { Part0, Part1 }
    public Part musicPart;
    
    public enum Segment { A, B, C }
    public Segment musicSegment;

    public MusicPart(AudioClip newMusic, Part newPart, Segment newSegment)
    {
        musicClip = newMusic;
        musicPart = newPart;
        musicSegment = newSegment;
    }
}
