using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<Sound> sounds;
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    void Start()
    {
        Play("Theme", "MusicEnabled");
    }

    public void Play(string name, string playerPrefName)
    {
        if (PlayerPrefs.GetInt(playerPrefName, 1) == 0)
        {
            if(playerPrefName == "MusicEnabled")
            {
                Stop("Theme");
            }
            return;
        }

        var soundToPlay = sounds.FirstOrDefault(s => s.name == name);
        if(soundToPlay == null)
        {
            Debug.LogWarning("Sound name not found!");
            return;
        }
        soundToPlay.source.Play();
    }

    public void Stop(string name)
    {
        var soundToStop = sounds.FirstOrDefault(s => s.name == name);
        if (soundToStop == null)
        {
            Debug.LogWarning("Sound name not found!");
            return;
        }
        soundToStop.source.Stop();
    }
}
