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

        // ������ Ÿ�� ������ �˻�, �������� ���� ���� �� ���� ���� �������� ���� ���ŵ�
        public bool IsCageTypeCell(int nRow, int nCol)
        {
            return false;
        }
    }
}
