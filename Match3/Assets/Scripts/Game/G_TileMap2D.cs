using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_TileMap2D : MonoBehaviour
{
    [Header("Tile")]
    [SerializeField] GameObject _tilePrefab;

    public void GenerateTileMap(MapData mapData)
    {
        int width = mapData._mapSize.x;
        int height = mapData._mapSize.y;

        for(int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {
                int index = y * width + x;   // 왼쪽 상단부터 오른쪽 하단까지 번호 지정 (0, 1, 2, ...)
                if(mapData._mapData[index] == (int)_eTileType.EMPTY)   // EMPTY 타일이면 생성하지 않음
                {
                    continue;
                }

                // 생성되는 타일맵의 중앙이 (0, 0, 0인 위치)
                Vector3 position = new Vector3((width * 0.5f * -1 + 0.5f) + x, (height * 0.5f - 0.5f) - y, 0);

                if (mapData._mapData[index] > (int)_eTileType.EMPTY)   // EMPTY 타일이 아니면 자리마다 타일 생성
                {
                    SpawnTile((_eTileType)mapData._mapData[index], position);
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
}
