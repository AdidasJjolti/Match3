using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    static BGMManager instance;

    public static BGMManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<BGMManager>();

                if (instance == null)
                {
                    return null;
                }
            }
            return instance;
        }
    }

    [SerializeField] AudioClip _bgmClip;
    [SerializeField] AudioClip[] _sfxClips;

    AudioSource _bgmPlayer;
    AudioSource _sfxPlayer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _sfxPlayer = GetComponent<AudioSource>();
        SetBGM();
        SetSFX();

        if (_bgmPlayer != null)
        {
            _bgmPlayer.Play();
            _bgmPlayer.loop = true;
        }

        Debug.Log($"BGM volume is {_bgmPlayer.volume}");
    }

    void SetBGM()
    {
        GameObject child = new GameObject("BGM");
        child.transform.parent = transform;
        _bgmPlayer = child.AddComponent<AudioSource>();
        _bgmPlayer.clip = _bgmClip;

        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            _bgmPlayer.volume = PlayerPrefs.GetFloat("BGMVolume");
        }
        else
        {
            _bgmPlayer.volume = 0.7f;
        }
    }

    void SetSFX()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            _sfxPlayer.volume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            _sfxPlayer.volume = 0.7f;
        }
    }

    public void PlayButtonClickSound()
    {
        _sfxPlayer.PlayOneShot(_sfxClips[0]);
    }

    public void SetBGMVolume(float volume)
    {
        _bgmPlayer.volume = volume;
    }
}
