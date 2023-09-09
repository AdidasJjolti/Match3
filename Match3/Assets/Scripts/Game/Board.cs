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

        internal void ComposeStage(Transform container = null)
        {
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
            return -_Row / 2.0f + offset;
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

            // ���ο� breed�� ����
            block.breed = genBreed;
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

            // ����Ʈ�� �ִ� �� ��� ����
            clearBlocks.ForEach((block) => block.Destroy());
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

            // baseBlock ���� ���� �� �˻�
            for (int i = nCol + 1; i < nCol; i++)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

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

                matchedBlockList.Insert(0, block);
            }

            if(matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, true);
                found = true;
            }

            matchedBlockList.Clear();

            matchedBlockList.Add(baseBlock);

            // baseBlock ���� ���� �� �˻�
            for (int i = nRow + 1; i < nRow; i++)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Add(block);
            }

            // baseBlock ���� �Ʒ��� �� �˻�
            for (int i = nRow - 1; i >= 0; i--)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Insert(0, block);
            }

            if (matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, false);
                found = true;
            }

            matchedBlockList.Clear();

            return found;
        }

        void SetBlockStatusMatched(List<Block> blockList, bool horz)        // 3��ġ ���� ��ġ�� ���θ� true, ���θ� false
        {
            int matchCount = blockList.Count;
            blockList.ForEach(block => block.UpdateBlockStatusMatched((_eMatchType)matchCount));
            
            // ���� ���ٽİ� ������ �ڵ�
            //foreach(var block in blockList)
            //{
            //    block.UpdateBlockStatusMatched((_eMatchType)matchCount);
            //}
        }

        public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
        {
            SortedList<int, int> tempBlocks = new SortedList<int, int>();       // �� ������ ���� ��ġ�� ������ �ӽ� ����Ʈ
            List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();            // �̵��� �ʿ��� �� ����Ʈ�� ������ ����Ʈ

            int emptyCount = 0;

            for(int nCol = 0; nCol < _Col; nCol++)
            {
                tempBlocks.Clear();

                for(int nRow = 0; nRow < _Row; nRow++)
                {
                    if(CanBlockBeAllocatable(nRow, nCol))
                    {
                        tempBlocks.Add(nRow, nRow);
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
                for(int nRow = first.Value + 1; nRow < _Row; nRow++)
                {
                    Block block = _blocks[nRow, nCol];

                    if(block == null || _cells[nRow, nCol].type == _eTileType.EMPTY)
                    {
                        continue;
                    }

                    block._dropDistance = new Vector2(0, nRow - first.Value);
                    movingBlocks.Add(block);

                    Debug.Assert(_cells[first.Value, nCol].IsObstracle() == false, $"{_cells[first.Value, nCol]}");
                    _blocks[first.Value, nCol] = block;

                    _blocks[nRow, nCol] = null;

                    emptyBlocks.RemoveAt(0);
                    emptyBlocks.Add(nRow, nCol);

                    first = emptyBlocks.First();
                    nRow = first.Value;
                }

                emptyCount = emptyBlocks.Count;
            }

            yield return null;

            if(emptyCount > 0)
            {
                unfilledBlocks.AddRange(emptyRemainBlocks);
            }

            yield break;
        }

        bool CanBlockBeAllocatable(int nRow, int nCol)
        {
            if(_cells[nRow, nCol].type.IsBlockAllocatableType())
            {
                return false;
            }

            return _blocks[nRow, nCol] == null;
        }
    }
}

