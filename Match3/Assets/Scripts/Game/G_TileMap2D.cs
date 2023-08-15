using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Board;

public class G_TileMap2D : MonoBehaviour
{
    [Header("Tile")]
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject _blockPrefab;
    int _width;
    int _height;


    public void GenerateTileMap(MapData mapData)
    {
        _width = mapData._mapSize.x;
        _height = mapData._mapSize.y;

        GetWidth();
        GetHeight();

        for(int y = 0; y < _height; ++y)
        {
            for(int x = 0; x < _width; ++x)
            {
                int index = y * _width + x;   // 왼쪽 상단부터 오른쪽 하단까지 번호 지정 (0, 1, 2, ...)
                if(mapData._mapData[index] == (int)_eTileType.EMPTY)   // EMPTY 타일이면 생성하지 않음
                {
                    continue;
                }

                // 생성되는 타일맵의 중앙이 (0, 0, 0인 위치)
                Vector3 position = new Vector3((_width * 0.5f * -1 + 0.5f) + x, (_height * 0.5f - 0.5f) - y, 0);

                Debug.Log("블럭 스폰할게");
                if (mapData._mapData[index] > (int)_eTileType.EMPTY)   // EMPTY 타일이 아니면 자리마다 타일 생성
                {
                    SpawnTile((_eTileType)mapData._mapData[index], position);
                    SpawnBlock(_eBlockType.EMPTY, position);
                }

                if(mapData._mapData[index] == (int)_eTileType.NORMAL)  // NORMAL 타일인 곳에만 블럭 생성
                {
                    SpawnBlock(_eBlockType.BASIC, position);
                }
            }
        }
    }

    void SpawnTile(_eTileType tileType, Vector3 position)
    {
        GameObject clone = Instantiate(_tilePrefab, position, Quaternion.identity);
        clone.name = "Tile";
        clone.transform.SetParent(transform);

        Tile tile = clone.GetComponent<Tile>();
        tile.Setup(tileType);     // 생성한 clone 오브젝트의 Setup 함수 호출
    }

    void SpawnBlock(_eBlockType blockType, Vector3 position)
    {
        GameObject clone = Instantiate(_blockPrefab, position, Quaternion.identity);
        clone.name = "Block";
        clone.transform.SetParent(transform);
        clone.GetComponent<Block>().type = blockType;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }
}
