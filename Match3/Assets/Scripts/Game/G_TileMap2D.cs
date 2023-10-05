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
                int index = y * _width + x;   // 왼쪽 상단부터 오른쪽 하단까지 번호 지정 (0, 1, 2, ...)
                if (mapData._mapData[index] == (int)_eTileType.EMPTY)   // EMPTY 타일이면 생성하지 않음
                {
                    continue;
                }

                // 생성되는 타일맵의 중앙이 (0, 0, 0인 위치)
                Vector3 position = new Vector3((_width * 0.5f * -1 + 0.5f) + x, (_height * 0.5f - 0.5f) - y, 0);

                if (mapData._mapData[index] > (int)_eTileType.EMPTY)   // EMPTY 타일이 아니면 자리마다 타일 생성
                {
                    SpawnTile((_eTileType)mapData._mapData[index], position, x, y);
                }

                if (mapData._mapData[index] == (int)_eTileType.NORMAL)  // NORMAL 타일인 곳에만 NORMAL 블럭 생성
                {
                    SpawnBlock(_eBlockType.BASIC, position, x, y);
                }
                else
                {
                    SpawnBlock(_eBlockType.EMPTY, position, x, y);
                }
            }
        }

        Debug.Log("생성 맵 블록 정보 출력 : ");
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
        Debug.Log("맵 생성 완료");
    }

    void SpawnTile(_eTileType tileType, Vector3 position, int x, int y)
    {
        GameObject clone = Instantiate(_tilePrefab, position, Quaternion.identity);
        clone.name = "Tile" + _IDXcell.ToString();
        clone.transform.SetParent(transform);

        Tile tile = clone.GetComponent<Tile>();
        tile.Setup(tileType);     // 생성한 clone 오브젝트의 Setup 함수 호출

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

        clone.GetComponent<BlockBehaviour>().UpdateView(false);
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

        clone.GetComponent<BlockBehaviour>().UpdateView(false);

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
}
