using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public enum _eClip
    {
        NONE = -1,
        CHOMP,
        BLOCKCLEAR,

        MAX
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager _instance;
        AudioSource[] _sfx;

        void Start()
        {
            _instance = GetComponent<SoundManager>();
            _sfx = GetComponents<AudioSource>();
        }

        public void PlayOneShot(_eClip audioClip)
        {
            _sfx[(int)audioClip].Play();
        }

        public void PlayOneShot(_eClip audioClip, float volumeScale)
        {
            AudioSource source = _sfx[(int)audioClip];
            source.PlayOneShot(source.clip, volumeScale);
        }
    }
}
