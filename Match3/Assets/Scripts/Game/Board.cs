using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class Board
    {
        int _row;
        int _col;
        Transform _container;

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
    }
}

