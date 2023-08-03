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

        // �÷����� �������� ���� ����
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

