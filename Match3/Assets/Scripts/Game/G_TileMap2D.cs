using Match3.Board;
using UnityEngine;

public class G_TileMap2D : MonoBehaviour
{
    [Header("Tile")]
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject _blockPrefab;
    int _width;
    int _height;

    int _IDXcell = 0;
    int _IDXblock = 0;

    GameObject[,] _tileMapCells;
    public GameObject[,] tileMapCells
    {
        get
        {
            return _tileMapCells;
        }
    }

    GameObject[,] _tileMapBlocks;
    public GameObject[,] tileMapBlocks
    {
        get
        {
            return _tileMapBlocks;
        }
    }

    public void GenerateTileMap(MapData mapData)
    {
        _width = mapData._mapSize.x;
        _height = mapData._mapSize.y;

        GetWidth();
        GetHeight();

        _tileMapCells = new GameObject[GetWidth(), GetHeight()];
        _tileMapBlocks = new GameObject[GetWidth(), GetHeight()];


        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                int index = y * _width + x;   // ���� ��ܺ��� ������ �ϴܱ��� ��ȣ ���� (0, 1, 2, ...)
                if (mapData._mapData[index] == (int)_eTileType.EMPTY)   // EMPTY Ÿ���̸� �������� ����
                {
                    continue;
                }

                // �����Ǵ� Ÿ�ϸ��� �߾��� (0, 0, 0�� ��ġ)
                Vector3 position = new Vector3((_width * 0.5f * -1 + 0.5f) + x, (_height * 0.5f - 0.5f) - y, 0);

                if (mapData._mapData[index] > (int)_eTileType.EMPTY)   // EMPTY Ÿ���� �ƴϸ� �ڸ����� Ÿ�� ����
                {
                    SpawnTile((_eTileType)mapData._mapData[index], position, x, y);
                }

                if (mapData._mapData[index] == (int)_eTileType.NORMAL)  // NORMAL Ÿ���� ������ NORMAL �� ����
                {
                    SpawnBlock(_eBlockType.BASIC, position, x, y);
                }
                else
                {
                    SpawnBlock(_eBlockType.EMPTY, position, x, y);
                }
            }
        }

        Debug.Log("���� �� ��� ���� ��� : ");
        System.Text.StringBuilder blocks = new System.Text.StringBuilder();

        for (int row = 0; row < _width; row++)
        {
            for(int col = 0; col < _height; col++)
            {
                blocks.Append($"{_tileMapBlocks[col, row].GetComponent<Block>()._breed}, ");
            }

            blocks.Append("\n");
        }

        Debug.Log(blocks.ToString());
        Debug.Log("�� ���� �Ϸ�");
    }

    void SpawnTile(_eTileType tileType, Vector3 position, int x, int y)
    {
        GameObject clone = Instantiate(_tilePrefab, position, Quaternion.identity);
        clone.name = "Tile" + _IDXcell.ToString();
        clone.transform.SetParent(transform);

        Tile tile = clone.GetComponent<Tile>();
        tile.Setup(tileType);     // ������ clone ������Ʈ�� Setup �Լ� ȣ��

        _tileMapCells[x, y] = clone;
        _IDXcell++;
    }

    void SpawnBlock(_eBlockType blockType, Vector3 position, int x, int y)
    {
        GameObject clone = Instantiate(_blockPrefab, position, Quaternion.identity);
        clone.name = "Block" + _IDXblock.ToString();
        clone.transform.SetParent(transform);
        var block = clone.GetComponent<Block>();
        block.Init(blockType);

        _tileMapBlocks[x, y] = clone;
        _IDXblock++;

        clone.GetComponent<BlockBehaviour>().UpdateView();
    }

    public Block RespawnBlock(int row, int col)
    {
        GameObject clone = Instantiate(_blockPrefab);
        clone.name = "Block" + _IDXblock.ToString();
        clone.transform.SetParent(transform);
        var block = clone.GetComponent<Block>();
        block.Init(_eBlockType.BASIC);

        _tileMapBlocks[row, col] = clone;
        _IDXblock++;

        clone.GetComponent<BlockBehaviour>().UpdateView();

        return clone.GetComponent<Block>();
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

#if UNITY_EDITOR

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F11))
        {
            var children = transform.GetComponentsInChildren<Transform>();
            int count = 0;
            foreach(var child in children)
            {
                if(child.name.Contains("Block"))
                {
                    count++;
                }
            }

            Debug.Log($"���� ����� ������ {count}��");
        }
    }

#endif
}
