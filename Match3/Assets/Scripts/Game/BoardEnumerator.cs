using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class BoardEnumerator
    {
        Match3.Board.Board _board;

        public BoardEnumerator(Match3.Board.Board board)
        {
            this._board = board;
        }

        // 케이지 타입 셀인지 검사, 케이지에 갇힌 블럭은 블럭 제거 전에 케이지가 먼저 제거됨
        public bool IsCageTypeCell(int nRow, int nCol)
        {
            return false;
        }
    }
}
