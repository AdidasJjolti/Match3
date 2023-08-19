using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _eTileType
{
    EMPTY = 0,
    GRASS,      // 여기는 블럭을 배치하지 않음, 사실상 EMPTY
    NORMAL,     // 여기 있는 블럭은 한 번만에 깰 수 있음
    HARD,       // 여기 있는 블럭은 깰 수 없음

    MAX
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

    // 블럭이 위치할 수 있는 타입인지 체크
    public bool IsBlockAllocatableType(_eTileType type)
    {
        return (type == _eTileType.NORMAL);
    }

    // 블럭이 이동 가능한 타입인지 체크
    public bool IsBlockMovableType(_eTileType type)
    {
        return (type == _eTileType.NORMAL);
    }
}
