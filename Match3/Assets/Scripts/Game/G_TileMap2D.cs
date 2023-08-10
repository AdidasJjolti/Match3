using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                int index = y * _width + x;   // ���� ��ܺ��� ������ �ϴܱ��� ��ȣ ���� (0, 1, 2, ...)
                if(mapData._mapData[index] == (int)_eTileType.EMPTY)   // EMPTY Ÿ���̸� �������� ����
                {
                    continue;
                }

                // �����Ǵ� Ÿ�ϸ��� �߾��� (0, 0, 0�� ��ġ)
                Vector3 position = new Vector3((_width * 0.5f * -1 + 0.5f) + x, (_height * 0.5f - 0.5f) - y, 0);

                if (mapData._mapData[index] > (int)_eTileType.EMPTY)   // EMPTY Ÿ���� �ƴϸ� �ڸ����� Ÿ�� ����
                {
                    SpawnTile((_eTileType)mapData._mapData[index], position);
                }

                if(mapData._mapData[index] == (int)_eTileType.NORMAL)
                {
                    SpawnBLock(position);
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
        tile.Setup(tileType);     // ������ clone ������Ʈ�� Setup �Լ� ȣ��
    }

    void SpawnBLock(Vector3 position)
    {
        GameObject clone = Instantiate(_blockPrefab, position, Quaternion.identity);
        clone.name = "Block";
        clone.transform.SetParent(transform);

        
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
