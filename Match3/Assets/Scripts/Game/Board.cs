using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;
using Util;

namespace Match3.Board
{
    using IntIntKV = KeyValuePair<int, int>;
    public class Board
    {
        int _row;
        int _col;
        Transform _container;
        G_TileMap2D _tileMap2D;

        BoardEnumerator _enumerator;

        // �÷����� �������� ��, �� ����
        public int _Row
        {
            get
            {
                return _row;
            }
        }
        public int _Col
        {
            get
            {
                return _col;
            }
        }

        // �������� �����ϴ� ���� �����ϴ� 2���� �迭 ����
        // �� : �� �Ʒ��� ��ġ�� �������� �ʴ� Ÿ�� �ϳ��ϳ�
        Cell[,] _cells;
        public Cell[,] cells
        {
            get
            {
                return _cells;
            }
        }

        // �������� �����ϴ� ���� �����ϴ� 2���� �迭 ����
        // �� : ������ �����Ͽ� 3��ġ�� ���� Ÿ��
        Block[,] _blocks;
        public Block[,] blocks
        {
            get
            {
                return _blocks;
            }
        }

        // row * col ũ��� �迭 ����
        public Board(int row, int col)
        {
            _row = row;
            _col = col;

            // Cell, Block ������ 2���� �迭�� ����
            _cells = new Cell[row, col];
            _blocks = new Block[row, col];

            _enumerator = new BoardEnumerator(this);
        }

        internal void ComposeStage(G_TileMap2D tilemap2D, Transform container = null)
        {
            _tileMap2D = tilemap2D;
            _container = container;
            
            BoardShuffler shuffler = new BoardShuffler(this, true);
            shuffler.Shuffle();
        }

        public float CalcInitX(float offset = 0)
        {
            return -_Col / 2.0f + offset;
        }

        public float CalcInitY(float offset = 0)
        {
            return _Row / 2.0f - offset;
        }

        // �ش� ��ġ ���� ���� ������� �˻�
        // ������ ��ġ�� NORMAL Ÿ�� ���� ��� true ��ȯ
        public bool CanShuffle(int nRow, int nCol, bool loading)
        {
            if (!_cells[nRow, nCol].type.IsBlockMovableType())
            {
                return false;
            }

            return true;
        }

        // �ش� ���� breed�� �ٸ� ������ ����
        public void ChangeBlock(Block block, _eBlockBreed notAllowedBreed)
        {
            _eBlockBreed genBreed;

            // notAllowedBreed�� �ߺ����� ���� ������ �ݺ�
            while (true)
            {
                genBreed = (_eBlockBreed)Random.Range(0, 6);
                if (notAllowedBreed == genBreed)
                {
                    continue;
                }
                break;
            }

            // ���ο� breed�� �����ϰ� ��������Ʈ�� ������ breed�� �°� ����
            block.breed = genBreed;
            block.blockBehaviour.ChangeView(genBreed);
        }

        public bool IsSwipeable(int nRow, int nCol)
        {
            return _cells[nRow, nCol].type.IsBlockMovableType();    // ������ �� �ִ� ���� ��� �������� �����ϹǷ� true ��ȯ
        }

        // 3��ġ ���� �Ǵ��Ͽ� �����ϴ� ���� ��Ģ ����
        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            bool matchedBlockFound = UpdateAllBlocksMathcedStatus();        // 3��ġ ���� ������ true ��ȯ

            if(matchedBlockFound == false)
            {
                matchResult.value = false;
                yield break;
            }

            List<Block> clearBlocks = new List<Block>();        // ���ŵ� ���� �����ϴ� ����Ʈ

            for(int nRow = 0; nRow < _Row; nRow++)
            {
                for(int nCol = 0; nCol < _Col; nCol++)
                {
                    Block block = _blocks[nRow, nCol];
                    block?.DoEvaluation(_enumerator, nRow, nCol);       // 3��ġ�� �Ǵ� ������ üũ

                    if(block != null)
                    {
                        // Ŭ���� ������ ���̸� ���ŵ� �� ����Ʈ�� �ְ� blocks �迭���� ����
                        if(block._status == _eBlockStatus.CLEAR)
                        {
                            clearBlocks.Add(block);
                            _blocks[nRow, nCol] = null;
                        }
                    }
                }
            }

            // 1. clearBlocks ����Ʈ�� ũ�� �Ǵ�
            // 2. 3�̸� ���� ���� �״�� ����, 4�̸� ������ ������ ����, 5 �̻��̸� ��ź ����
            // 3. clearBlocks ����Ʈ�� ũ�Ⱑ 4 �Ǵ� 5�� ��� ����Ʈ �� ���� ��� �����ϱ� ���� �ϳ��� �������� �̾� clearBlocks���� ����
            // 4. ����Ʈ���� ������ ����� ������ ������� ��� ����, �Ӽ� ����
            // 5. ���Ŀ��� clearBlocks ����Ʈ �� ���� ���� ��� �����ϴ� �ڵ� ����

            // ���� 1 : ����Ʈ���� ���� ����� ��������Ʈ�� ������� ����
            // �ذ� ��� : ��������Ʈ ���� ������ �����Ͽ� ���������� �ٲٵ��� ��
            // �ذ� �Ϸ�

            // ���� 2 : ����Ʈ���� ���� ����� ���ڸ����� �ӹ����ֱ⸸�ϰ� �� �ڸ��� ������� ����
            // �ذ� ��� : ArrangeBlocksAfterClean���� �ش� ����� �����ϰ� �ִ��� Ȯ�� �ʿ�
            // �ذ� �Ϸ� : clearBlocks�� add�� �� �̹� block�� null ó���� ���� ������ ������� ����� ��� _blocks �迭�� �ٽ� ä������

            // ���� 3 : ���� �ٸ� �� ������ ����� ���� 3��, 4�� ���ŵǴ� ��� ������ ����� 4�� ���ŵǴ� ��� �߿��� �����ؾ� ��
            // �ذ� ��� : ���� �ٸ� �� ������ ����� ���ŵǴ� ��� _eMatchType�� THREE�� �ƴ� ����� �ִ��� üũ�Ͽ� �ش� ��ϵ� �߿����� �������� 1���� ��� ������ ���� ���� ����

            int listSize = clearBlocks.Count;

            bool isSameBlock = true;

            // ������ ��� ���� ���� 1
            // ��� ���� ������ ����̰� clearBlocks�� ũ�Ⱑ 3���� ũ�� ������ ��� ���� �ڵ� ����
            for(int i = 1; i < listSize; i++)
            {
                if(clearBlocks[0].breed != clearBlocks[i].breed)
                {
                    isSameBlock = false;
                    break;
                }
            }

            if (listSize > 3 && isSameBlock)
            {
                // ����Ʈ���� ������ ����� �����Ͽ� ��� ��ġ ������ �´� ���������� ����
                int num = Random.Range(0, listSize);
                clearBlocks[num].type = _eBlockType.ITEM;
                clearBlocks[num].blockBehaviour.UpdateView(listSize);
                clearBlocks[num].durability = 1;
                clearBlocks[num]._match = _eMatchType.NONE;
                clearBlocks[num]._questType = _eBlockQuestType.CLEAR_SIMPLE;
                clearBlocks[num]._status = _eBlockStatus.NORMAL;

                // ������ ������ ����� _blocks �迭�� �ٽ� �Է�
                float row = clearBlocks[num].blockObj.localPosition.x + 4.5f;
                float col = 4.5f - clearBlocks[num].blockObj.localPosition.y;
                _blocks[(int)row, (int)col] = clearBlocks[num].GetComponent<Block>();

                clearBlocks.RemoveAt(num);
            }

            // ������ ���� ���� 2
            // ���� �ٸ� �� ������ ����̸� _eMatchType �˻��Ͽ� THREE�� �ƴ� ����� �ִ��� Ȯ��
            // _eMatchType�� THREE�� �ƴ� ����� �ִٸ� �ش� ��ϵ� �߿��� ������ ����� ����

            List<int> reserveBlocks = new List<int>();  // _eMatchType�� THREE�� �ƴ� ����� ��ġ�� ������ ����Ʈ
            int idx = 0;                                // ������ ����� ������ clearBlocks�� �ε��� ����

            if (!isSameBlock)
            {
                for(int i = 0; i < clearBlocks.Count; i++)
                {
                    if(clearBlocks[i]._match == _eMatchType.FOUR || clearBlocks[i]._match == _eMatchType.FIVE)
                    {
                        reserveBlocks.Add(i);
                    }
                }

                if(reserveBlocks.Count > 0)
                {
                    idx = reserveBlocks[Random.Range(0, reserveBlocks.Count)];  // _eMatchType�� THREE�� �ƴ� ��� �� 1���� ���� clearBlocks ����Ʈ �� �ε���

                    clearBlocks[idx].type = _eBlockType.ITEM;
                    clearBlocks[idx].blockBehaviour.UpdateView(reserveBlocks.Count);
                    clearBlocks[idx].durability = 1;
                    clearBlocks[idx]._match = _eMatchType.NONE;
                    clearBlocks[idx]._questType = _eBlockQuestType.CLEAR_SIMPLE;
                    clearBlocks[idx]._status = _eBlockStatus.NORMAL;

                    // ������ ������ ����� _blocks �迭�� �ٽ� �Է�
                    float row = clearBlocks[idx].blockObj.localPosition.x + 4.5f;
                    float col = 4.5f - clearBlocks[idx].blockObj.localPosition.y;
                    _blocks[(int)row, (int)col] = clearBlocks[idx].GetComponent<Block>();

                    clearBlocks.RemoveAt(idx);
                }
            }

            // ����Ʈ�� �ִ� �� ��� ����
            clearBlocks.ForEach((block) => block.Destroy());

            yield return new WaitForSeconds(0.15f);         // ���� ���ŵǴ� ���� ������ ����

            matchResult.value = true;

            yield break;
        }

        // 3��ġ ���� �ִ��� üũ
        public bool UpdateAllBlocksMathcedStatus()
        {
            List<Block> matchedBlockList = new List<Block>();
            int count = 0;

            for (int nRow = 0; nRow < _Row; nRow++)
            {
                for (int nCol = 0; nCol < _Col; nCol++)
                {
                    if (EvalBlocksIfMatched(nRow, nCol, matchedBlockList))
                    {
                        count++;    // ��ġ�Ǵ� ���� ���涧���� count ����
                    }
                }
            }

            return count > 0;
        }

        // ���� �Ǵ� ���η� �� �˻��Ͽ� ��ġ�Ǵ� ���� �ϳ��� ������ true ��ȯ
        public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
        {
            bool found = false;
            Block baseBlock = _blocks[nRow, nCol];

            if(baseBlock == null)
            {
                return false;
            }

            if(baseBlock._match != _eMatchType.NONE || !baseBlock.IsValidate() || _cells[nRow, nCol].IsObstracle())
            {
                return false;
            }

            matchedBlockList.Add(baseBlock);

            Block block;

            // baseBlock ���� �Ʒ��� �� �˻�
            for (int i = nCol + 1; i < _Col; i++)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}�� ��ġ�� ��� ���� �Ʒ��� ��ϰ� ��ġ");
                matchedBlockList.Add(block);
            }

            // baseBlock ���� ���� �� �˻�
            for (int i = nCol - 1; i >= 0; i--)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}�� ��ġ�� ��� ���� ���� ��ϰ� ��ġ");
                matchedBlockList.Insert(0, block);
            }

            if(matchedBlockList.Count >= 3)
            {
                foreach (var item in matchedBlockList)
                {
                    Debug.Log($"{item.blockObj.localPosition.x}, {item.blockObj.localPosition.y}�� ��ġ�� ����� ����Ʈ�� �߰���");
                }

                SetBlockStatusMatched(matchedBlockList, false);  // ���� ��ġ �߻�
                found = true;
            }

            matchedBlockList.Clear();

            matchedBlockList.Add(baseBlock);

            // baseBlock ���� ������ �� �˻�
            for (int i = nRow + 1; i < _Row; i++)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}�� ��ġ�� ��� ���� ������ ��ϰ� ��ġ");
                matchedBlockList.Add(block);
            }

            // baseBlock ���� ���� �� �˻�
            for (int i = nRow - 1; i >= 0; i--)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}�� ��ġ�� ��� ���� ���� ��ϰ� ��ġ");
                matchedBlockList.Insert(0, block);
            }

            if (matchedBlockList.Count >= 3)
            {
                foreach (var item in matchedBlockList)
                {
                    Debug.Log($"{item.blockObj.localPosition.x}, {item.blockObj.localPosition.y}�� ��ġ�� ����� ����Ʈ�� �߰���");
                }

                SetBlockStatusMatched(matchedBlockList, true);  // ���� ��ġ �߻�
                found = true;
            }

            matchedBlockList.Clear();

            return found;
        }

        void SetBlockStatusMatched(List<Block> blockList, bool horz)        // 3��ġ ���� ��ġ�� ���θ� true, ���θ� false
        {
            int matchCount = blockList.Count;
            blockList.ForEach(block => block.UpdateBlockStatusMatched((_eMatchType)matchCount));
            
            foreach(var block in blockList)
            {
                switch (block._match)
                {
                    case _eMatchType.THREE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 3�� ��� ��Ī��");
                        break;
                    case _eMatchType.FOUR:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 4�� ��� ��Ī��");
                        break;
                    case _eMatchType.FIVE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 5�� ��� ��Ī��");
                        break;
                    case _eMatchType.THREE_THREE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 3+3�� ��� ��Ī��");
                        break;
                    case _eMatchType.THREE_FOUR:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 3+4�� ��� ��Ī��");
                        break;
                    case _eMatchType.THREE_FIVE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 3+5�� ��� ��Ī��");
                        break;
                    case _eMatchType.FOUR_FIVE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 4+5�� ��� ��Ī��");
                        break;
                    case _eMatchType.FOUR_FOUR:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}�� ��ġ�� ����� �������ͽ� : 4+4�� ��� ��Ī��");
                        break;
                }
            }

            // ���� ���ٽİ� ������ �ڵ�
            //foreach(var block in blockList)
            //{
            //    block.UpdateBlockStatusMatched((_eMatchType)matchCount);
            //}
        }

        public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
        {
            Debug.Log("ArrangeBlocksAfterClean ����");
            SortedList<int, int> tempBlocks = new SortedList<int, int>();       // �� ������ ���� ��ġ�� ������ �ӽ� ����Ʈ
            List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();            // �̵��� �ʿ��� �� ����Ʈ�� ������ ����Ʈ

            for(int nRow = 0; nRow < _Row; nRow++)
            {
                tempBlocks.Clear();

                for(int nCol = 0; nCol < _Col; nCol++)
                {
                    if(CanBlockBeAllocatable(nRow, nCol))
                    {
                        tempBlocks.Add(nCol, nCol);
                    }
                }

                IComparer<int> reverseComparer = new ReverseComparer<int>();
                SortedList<int, int> emptyBlocks = new SortedList<int, int>(tempBlocks, reverseComparer);      // �� ������ ���� ��ġ�� ������ ����Ʈ

                if (emptyBlocks.Count == 0)
                {
                    continue;
                }

                // ���� �Ʒ��� ����ִ� �� ó��
                IntIntKV first = emptyBlocks.First();

                // ����ִ� �� ���� �������� �̵� ������ ���� Ž���ϸ鼭 �� �� �ڸ��� ä��
                for(int col = first.Value - 1; col >= 0; col--)
                {
                    Block block = _blocks[nRow, col];

                    if(block == null || _cells[nRow, col].type != _eTileType.NORMAL)
                    {
                        continue;
                    }

                    // �̵��� �� �߰�
                    block._dropDistance = new Vector2(0, col - first.Value);
                    movingBlocks.Add(block);

                    Debug.Assert(_cells[first.Value, col].IsObstracle() == false, $"{_cells[first.Value, col]}");
                    // �̵��� ��ġ�� board���� ����� ��ġ �̵�
                    _blocks[nRow, first.Value] = block;
                    // �ٸ� ������ �̵��Ͽ� ���� ��ġ �����
                    _blocks[nRow, col] = null;

                    emptyBlocks.RemoveAt(0);
                    emptyBlocks.Add(col, col);

                    first = emptyBlocks.First();
                    col = first.Value;
                }
            }

            yield return null;

            if(emptyRemainBlocks.Count > 0)
            {
                unfilledBlocks.AddRange(emptyRemainBlocks);
            }

            yield break;
        }

        bool CanBlockBeAllocatable(int nRow, int nCol)
        {
            if(!_cells[nRow, nCol].type.IsBlockAllocatableType())
            {
                return false;
            }

            return _blocks[nRow, nCol] == null;
        }

        public IEnumerator SpawnBlocksAfterClean(List<Block> movingBlocks)
        {
            Debug.Log("SpawnBlocksAfterClean ����");
            for (int row = 0; row < _Row; row++)
            {
                for(int col = _Col - 1; col >= 0; col--)
                {
                    if(_blocks[row, col] == null)   // �ش� ��ġ�� ���� ������ ��� �Ʒ� �ڵ� ����
                    {
                        //int topCol = _Col;
                        // ���� �����Ǵ� ���� ����, 0�� ���� ��� �������� ù��° ���� �����Ǵ� ��ġ
                        // �� ���� �����Ǹ� 1�� �����ؼ� ���� ���� ���� ���� ���ʿ��� ����� �����ϵ��� ��
                        int spawnBaseY = 0;

                        for(int y = col; y >= 0; y--)   // y�� ���� ������ ��ǥ
                        {
                            if(_blocks[row, y] != null || !CanBlockBeAllocatable(row, y))
                            {
                                continue;
                            }

                            Block block = SpawnBlockWithDrop(row, y, row, spawnBaseY);
                            if(block != null)
                            {
                                movingBlocks.Add(block);
                            }

                            //spawnBaseY--;
                        }

                        break;
                    }
                }
            }

            yield return null;
        }

        // row, col : �� ���� �� ����Ǵ�(�����ϴ�) ��ġ
        // spawnedRow, spawnedCol : ���� �����Ǵ� ��ġ, row, col���� ���
        Block SpawnBlockWithDrop(int row, int col, int spawnedRow, int spawnedCol)
        {
            float initX = CalcInitX(Core.Constants.BLOCK_ORG);
            float initY = CalcInitY(Core.Constants.BLOCK_ORG);

            //Block block = _stageBuilder.SpawnBlock().InstantiateBlockObj(_blockPrefab, _container);
            Block block = _tileMap2D.RespawnBlock(row, col);
            if (block != null)
            {
                _blocks[row, col] = block;
                block.Move(initX + (float)spawnedRow, initY + (float)spawnedCol);
                block._dropDistance = new Vector2(0, spawnedCol - col);
            }

            return block;
        }
    }
}

