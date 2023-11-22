using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public enum _eClip
    {
        NONE = -1,
        CHOMP,
        BLOCKCLEAR,
        BGM,

        MAX
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager _instance;

        public static SoundManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<SoundManager>();

                    if (_instance == null)
                    {
                        return null;
                    }
                }
                return _instance;
            }
        }



        AudioSource[] _sfx;
        bool _isMute;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            //_instance = GetComponent<SoundManager>();
            _sfx = GetComponents<AudioSource>();
            _sfx[(int)_eClip.BGM].Play();
            _sfx[(int)_eClip.BGM].loop = true;
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

        public void SetMusicVolume()
        {
            _isMute = !_isMute;

            if(_isMute)
            {
                foreach(var audio in _sfx)
                {
                    audio.volume = 0;
                }
            }
            else
            {
                foreach (var audio in _sfx)
                {
                    audio.volume = 1;
                }
            }
        }

        public bool GetMuteState()
        {
            return _isMute;
        }
    }
}
