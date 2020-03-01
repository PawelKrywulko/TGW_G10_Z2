using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [Serializable]
    public class Sound
    {
        public string name;
        public bool loop;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume;
        [Range(0.1f, 3f)] public float pitch = 1f;

        [HideInInspector]
        public AudioSource source;
    }
}
