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

        internal void ComposeStage(Transform container = null)
        {
            _container = container;
            
            //for(int i = 0; i < _Row; i++)
            //{
            //    for (int j = 0; j < _Col; j++)
            //    {
            //        if(_blocks[i,j].GetComponent<Block>())
            //        {
            //            Debug.Log($"{i}, {j} : Valid Block");
            //        }
            //    }
            //}
            
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

        // 해당 위치 블럭이 셔플 대상인지 검사
        // 지정된 위치에 NORMAL 타입 셀인 경우 true 반환
        public bool CanShuffle(int nRow, int nCol, bool loading)
        {
            if (!_cells[nRow, nCol].type.IsBlockMovableType())
            {
                return false;
            }

            return true;
        }

        // 해당 블럭의 breed를 다른 값으로 변경
        public void ChangeBlock(Block block, _eBlockBreed notAllowedBreed)
        {
            _eBlockBreed genBreed;

            // notAllowedBreed와 중복되지 않을 때까지 반복
            while (true)
            {
                genBreed = (_eBlockBreed)Random.Range(0, 6);
                if (notAllowedBreed == genBreed)
                {
                    continue;
                }
                break;
            }

            // 새로운 breed를 설정
            block.breed = genBreed;
        }
    }
}

