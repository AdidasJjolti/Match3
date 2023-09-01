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
        // 플레이할 게임판의 행, 열 저장
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
        
        public bool IsOnValideBlock(Vector2 point, out BlockPos blockPos)       // point : 보드 기준 로컬 좌표, blockPos : point 좌표에 위치한 블럭의 위치 정보를 호출자에게 전달
        {
            // 로컬 좌표를 보드의 블럭 인덱스로 변환
            Vector2 pos = new Vector2(point.x + (_Col / 2.0f), point.y + (_Row / 2.0f));
            int nRow = (int)pos.y;
            int nCol = (int)pos.x;

            blockPos = new BlockPos(nRow, nCol);        // 호출자에게 전달할 out 파라미터 설정

            return _board.IsSwipeable(nRow, nCol);      // 스와이프 가능한 유효한 블럭인지 판단
        }

        public bool IsInsideBoard(Vector2 ptOrg)        // ptOrg : 보드 기준 로컬 좌표
        {
            // 10x10 보드인 경우, 0 ~ 10 사이의 좌표로 변환
            Vector2 point = new Vector2(ptOrg.x + (_Col / 2.0f), ptOrg.y + (_Row / 2.0f));

            if(point.y < 0 || point.x < 0 || point.y > _Row || point.x > _Col)
            {
                return false;
            }

            return true;
        }
    }
}

