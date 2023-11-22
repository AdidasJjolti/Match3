using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject _muteIcon;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        bool isMute = SoundManager.Instance.GetMuteState();

        if (isMute)
        {
            _muteIcon.SetActive(true);
        }
        else
        {
            _muteIcon.SetActive(false);
        }
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickSoundButton()
    {
        SoundManager.Instance.SetMusicVolume();
        bool isMute = SoundManager.Instance.GetMuteState();

        if(isMute)
        {
            _muteIcon.SetActive(true);
        }
        else
        {
            _muteIcon.SetActive(false);
        }
    }
}
