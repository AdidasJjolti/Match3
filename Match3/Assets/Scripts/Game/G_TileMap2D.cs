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
                int index = y * width + x;   // ���� ��ܺ��� ������ �ϴܱ��� ��ȣ ���� (0, 1, 2, ...)
                if(mapData._mapData[index] == (int)_eTileType.EMPTY)   // EMPTY Ÿ���̸� �������� ����
                {
                    continue;
                }

                // �����Ǵ� Ÿ�ϸ��� �߾��� (0, 0, 0�� ��ġ)
                Vector3 position = new Vector3((width * 0.5f * -1 + 0.5f) + x, (height * 0.5f - 0.5f) - y, 0);

                if (mapData._mapData[index] > (int)_eTileType.EMPTY)   // EMPTY Ÿ���� �ƴϸ� �ڸ����� Ÿ�� ����
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
        tile.Setup(tileType);     // ������ clone ������Ʈ�� Setup �Լ� ȣ��
    }
}
