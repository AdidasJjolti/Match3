using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Board;



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
            _tileType = value;   // �ܺο��� �޾ƿ� value�� �ʱ�ȭ
            _renderer.sprite = _tileImages[(int)_tileType];
        }
        get => _tileType;
    }
}
