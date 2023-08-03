using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class Board
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

        // 게임판을 구성하는 셀을 저장하는 2차원 배열 선언
        // 셀 : 블럭 아래에 위치한 움직이지 않는 타일 하나하나
        Cell[,] _cells;
        public Cell[,] cells
        {
            get
            {
                return _cells;
            }
        }

        // 게임판을 구성하는 블럭을 저장하는 2차원 배열 선언
        // 블럭 : 실제로 조작하여 3매치를 맞출 타일
        Block[,] _blocks;
        public Block[,] blocks
        {
            get
            {
                return _blocks;
            }
        }

        // row * col 크기로 배열 생성
        public Board(int row, int col)
        {
            _row = row;
            _col = col;

            // Cell, Block 각각의 2차원 배열을 생성
            _cells = new Cell[row, col];
            _blocks = new Block[row, col];
        }
    }
}

