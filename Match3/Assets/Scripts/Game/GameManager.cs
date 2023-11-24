using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Match3.Stage;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    return null;
                }
            }
            return _instance;
        }
    }

    [SerializeField] int _remainingMoves;
    [SerializeField] int _remainingBlocks;
    [SerializeField] StageController _stageController;
    [SerializeField] float _wait = 0.7f;
    [SerializeField] TextMeshProUGUI _txtMoves;
    [SerializeField] TextMeshProUGUI _txtBlocks;
    bool _isCoroutine;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetNumbers();
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


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
        SetNumbers();
    }

    public void SetNumbers()
    {
        _remainingMoves = 5;
        _remainingBlocks = 15;

        if(_txtMoves == null)
        {
            _txtMoves = GameObject.Find("Canvas").transform.Find("RemainingMoves").transform.Find("Background").Find("MovesCount").GetComponent<TextMeshProUGUI>();
        }

        if(_txtBlocks == null)
        {
            _txtBlocks = GameObject.Find("Canvas").transform.Find("RemainingBlocks").transform.Find("Background").Find("BlocksCount").GetComponent<TextMeshProUGUI>();
        }

        _txtMoves.text = _remainingMoves.ToString();
        _txtBlocks.text = _remainingBlocks.ToString();
    }

    public void ReduceRemainingMoves()
    {
        _remainingMoves--;
        _txtMoves.text = _remainingMoves.ToString();

        if (_remainingMoves < 1)
        {
            if(_remainingBlocks > 0)
            {
                Debug.Log("Game Over");
                // ToDo : 게임 오버 시 처리
                // 게임 오버 팝업, 타이틀로 이동 or 게임 재시작, 게임 재시작할 때 남은 움직임, 남은 블록 초기화
                if(_stageController == null)
                {
                    _stageController = FindObjectOfType<StageController>();
                }

                StartCoroutine(Wait(true));
            }
        }
    }

    public void ReduceRemainingBlocks(int deletedBlocks)
    {
        _remainingBlocks -= deletedBlocks;

        if(_remainingBlocks >= 0)
        {
            _txtBlocks.text = _remainingBlocks.ToString();
        }
        else
        {
            _txtBlocks.text = 0.ToString();
        }

        if (_remainingBlocks < 1)
        {
            Debug.Log("Game Clear");
            // ToDo : 게임 클리어 시 처리
            // 게임 클리어 팝업, 타이틀로 이동 or 게임 재시작, 게임 재시작할 때 남은 움직임, 남은 블록 초기화
            if (_stageController == null)
            {
                _stageController = FindObjectOfType<StageController>();
            }

            StartCoroutine(Wait(false));
        }
    }

    IEnumerator Wait(bool isGameOver)
    {
        if(_isCoroutine)
        {
            yield break;
        }

        _isCoroutine = true;

        Debug.Log("코루틴 시작");
        yield return new WaitForSecondsRealtime(_wait);

        if(isGameOver)
        {
            _stageController.OpenGameOverUI();
        }
        else
        {
            _stageController.OpenGameClearUI();
        }
        _isCoroutine = false;
    }
}
