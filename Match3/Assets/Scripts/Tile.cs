using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _eTileType
{
    EMPTY = 0,
    GRASS,
    NORMAL,
    HARD
}

public class Tile : MonoBehaviour
{
    [SerializeField] Sprite[] _tileImages;
    _eTileType _tileType;
    SpriteRenderer _renderer;

    public void Setup(_eTileType type)
    {
        _renderer = GetComponent<SpriteRenderer>();
        TileType = type;
    }

    public _eTileType TileType
    {
        set
        {
            _tileType = value;   // 외부에서 받아온 value를 초기화
            _renderer.sprite = _tileImages[(int)_tileType];
        }
        get => _tileType;
    }
}
