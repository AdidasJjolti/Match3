using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _remainingBlocks = 50;
    }

    public void ReduceRemainingMoves()
    {
        _remainingMoves--;

        if(_remainingMoves < 0)
        {
            _remainingMoves = 0;
        }
    }

    public void ReduceRemainingBlocks(int deletedBlocks)
    {
        _remainingBlocks -= deletedBlocks;

        if (_remainingBlocks < 0)
        {
            _remainingBlocks = 0;
        }
    }
}
