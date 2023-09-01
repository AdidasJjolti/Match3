using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Match3.Board;
using Util;

namespace Match3.Stage
{
    public class Stage
    {
        // �÷����� �������� ��, �� ����
        public int _Row
        {
            get
            {
                return _board._Row;
            }
        }
        public int _Col
        {
            get
            {
                return _board._Col;
            }
        }

        // �÷����� �������� ���� ����
        Match3.Board.Board _board;
        public Match3.Board.Board board
        {
            get
            {
                return _board;
            }
        }

        StageBuilder _stageBuilder;

        public Block[,] blocks
        {
            get
            {
                return _board.blocks;
            }
        }

        public Cell[,] cells
        {
            get
            {
                return _board.cells;
            }
        }

        public Stage(StageBuilder stageBuilder, int row, int col)
        {
            _stageBuilder = stageBuilder;
            _board = new Match3.Board.Board(row, col);
        }

        
        public void PrintAll()
        {
            System.Text.StringBuilder strCells = new System.Text.StringBuilder();
            System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

            for(int nRow = _Row - 1; nRow >= 0; nRow--)
            {
                for (int nCol = 0; nCol < _Col; nCol++)
                {
                    strCells.Append($"{cells[nRow, nCol].type}, ");
                    strBlocks.Append($"{blocks[nRow, nCol].type}, ");
                }

                strCells.Append("\n");
                strBlocks.Append("\n");
            }

            Debug.Log(strCells.ToString());
            Debug.Log(strBlocks.ToString());
        }
        
        public bool IsOnValideBlock(Vector2 point, out BlockPos blockPos)       // point : ���� ���� ���� ��ǥ, blockPos : point ��ǥ�� ��ġ�� ���� ��ġ ������ ȣ���ڿ��� ����
        {
            // ���� ��ǥ�� ������ �� �ε����� ��ȯ
            Vector2 pos = new Vector2(point.x + (_Col / 2.0f), point.y + (_Row / 2.0f));
            int nRow = (int)pos.y;
            int nCol = (int)pos.x;

            blockPos = new BlockPos(nRow, nCol);        // ȣ���ڿ��� ������ out �Ķ���� ����

            return _board.IsSwipeable(nRow, nCol);      // �������� ������ ��ȿ�� ������ �Ǵ�
        }

        public bool IsInsideBoard(Vector2 ptOrg)        // ptOrg : ���� ���� ���� ��ǥ
        {
            // 10x10 ������ ���, 0 ~ 10 ������ ��ǥ�� ��ȯ
            Vector2 point = new Vector2(ptOrg.x + (_Col / 2.0f), ptOrg.y + (_Row / 2.0f));

            if(point.y < 0 || point.x < 0 || point.y > _Row || point.x > _Col)
            {
                return false;
            }

            return true;
        }
    }
}

