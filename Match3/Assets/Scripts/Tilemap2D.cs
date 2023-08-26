using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Match3.Board;

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
        // ��ǲ �ʵ忡�� �Է��� width, height ������ �� ����
        int width, height;

        // TryParse �Լ��� �⺻ ��ȯ ���� bool������ out ���ϰ����� ����ȯ�Ͽ� ��ȯ ����
        int.TryParse(_inputWidth.text, out width);
        int.TryParse(_inputHeight.text, out height);

        _width = width;
        _height = height;

        for(int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                // �����Ǵ� Ÿ�ϸ��� �߾��� (0, 0, 0�� ��ġ)
                Vector3 position = new Vector3((_width * 0.5f * -1 + 0.5f) + x, (_height * 0.5f - 0.5f) - y, 0);
                SpawnTile(_eTileType.EMPTY, position);   // EMPTY Ÿ���� ����, �ʺ�ŭ ����
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
        tile.Setup(tileType);     // ������ clone ������Ʈ�� Setup �Լ� ȣ��

        _tileList.Add(tile);      // ������ Ÿ���� ����Ʈ�� �ϳ��� ����
    }

    public MapData GetMapData()
    {
        for(int i = 0; i < _tileList.Count; i++)
        {
            _mapData._mapData[i] = (int)_tileList[i].TileType;   // ���� ��� Ÿ�� ������ _mapData�� ����
        }

        return _mapData;
    }
}
