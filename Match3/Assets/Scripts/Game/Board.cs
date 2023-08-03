using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class Board
    {
        int _row;
        int _col;

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
    }
}

