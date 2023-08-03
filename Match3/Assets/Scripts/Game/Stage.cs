using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}

