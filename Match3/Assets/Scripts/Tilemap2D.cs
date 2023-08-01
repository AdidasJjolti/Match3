using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tilemap2D : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] TMP_InputField _inputWidth;
    [SerializeField] TMP_InputField _inputHeight;

    public int _width { private set; get; } = 10;
    public int _height { private set; get; } = 10;

    MapData _mapData;
    public List<Tile> _tileList { private set; get; }

    void Awake()
    {
        _inputWidth.text = _width.ToString();
        _inputHeight.text = _height.ToString();

        _mapData = new MapData();
        _tileList = new List<Tile>();
    }

    public void GenerateTilemap()
    {
        // 인풋 필드에서 입력한 width, height 값으로 맵 생성
        int width, height;

        // TryParse 함수의 기본 반환 값은 bool이지만 out 리턴값으로 형변환하여 반환 가능
        int.TryParse(_inputWidth.text, out width);
        int.TryParse(_inputHeight.text, out height);

        _width = width;
        _height = height;

        for(int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                // 생성되는 타일맵의 중앙이 (0, 0, 0인 위치)
                Vector3 position = new Vector3((_width * 0.5f * -1 + 0.5f) + x, (_height * 0.5f - 0.5f) - y, 0);
                SpawnTile(_eTileType.EMPTY, position);   // EMPTY 타일을 높이, 너비만큼 생성
            }
        }

        _mapData._mapSize.x = _width;
        _mapData._mapSize.y = _height;
        _mapData._mapData = new int[_tileList.Count];
    }

    void SpawnTile(_eTileType tileType, Vector3 position)
    {
        GameObject clone = Instantiate(_tilePrefab, position, Quaternion.identity);
        clone.name = "Tile";
        clone.transform.SetParent(transform);

        Tile tile = clone.GetComponent<Tile>();
        tile.Setup(tileType);     // 생성한 clone 오브젝트의 Setup 함수 호출

        _tileList.Add(tile);      // 생성한 타일을 리스트에 하나씩 저장
    }

    public MapData GetMapData()
    {
        for(int i = 0; i < _tileList.Count; i++)
        {
            _mapData._mapData[i] = (int)_tileList[i].TileType;   // 맵의 모든 타일 정보를 _mapData에 저장
        }

        return _mapData;
    }
}
