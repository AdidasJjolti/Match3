using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Match3.Board;

namespace Match3.Stage
{
    public class Stage
    {
        int _row;
        int _col;

        // 플레이할 게임판의 행, 열 저장
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

        // 플레이할 게임판의 변수 선언
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
            _row = row;
            _col = col;
        }

        public void PrintAll()
        {
            System.Text.StringBuilder strCells = new System.Text.StringBuilder();
            System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

            for(int nRow = _Row - 1; nRow >= 0; nRow--)
            {
                for (int nCol = 0; nRow < _Col; nCol++)
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
    }
}

