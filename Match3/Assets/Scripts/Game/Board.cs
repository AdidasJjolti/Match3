using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;
using Util;

namespace Match3.Board
{
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

        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            bool matchedBlockFound = UpdateAllBlocksMathcedStatus();        // 3��ġ ���� ������ true ��ȯ

            if(matchedBlockFound == false)
            {
                matchResult.value = false;
                yield break;
            }

            for(int nRow = 0; nRow < _Row; nRow++)
            {
                for(int nCol = 0; nCol < _Col; nCol++)
                {
                    Block block = _blocks[nRow, nCol];
                    block?.DoEvaluation(_enumerator, nRow, nCol);

                    if(block != null)
                    {
                        if(block.status == BlockStatus.CLEAR)
                        {
                            clearBlocks.Add(block);
                            _blocks[nRow, nCol] == null;
                        }
                    }
                }
            }

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
                        count++;
                    }
                }
            }

            return count > 0;
        }
    }
}

