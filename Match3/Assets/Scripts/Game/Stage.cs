using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Match3.Board;
using Util;
using Match3.Core;

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
            Vector2 pos = new Vector2(point.x + (_Col / 2.0f), point.y * -1 + (_Row / 2.0f));
            int nRow = (int)pos.x;
            int nCol = (int)pos.y;

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

        public IEnumerator CoDoSwipeAction(int nRow, int nCol, _eSwipe swipeDir, Returnable<bool> actionResult)
        {
            actionResult.value = false;     // 코루틴 리턴값 리셋

            // 스와이프되는 블럭의 위치 구하기
            int nSwipeRow = nRow, nSwipeCol = nCol;
            nSwipeRow += swipeDir.GetTargetCol();
            nSwipeCol += swipeDir.GetTargetRow();

            Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, $"Invalid Swipe : ({nSwipeRow}, {nSwipeCol})");        // 스와이프 되는 블럭의 위치가 클릭 위치와 같은 경우 실행하지 않음
            Debug.Assert(nSwipeRow >= 0 && nSwipeRow < _Row && nSwipeCol >= 0 && nSwipeCol < _Col, $"Swipe 타겟 블럭 인덱스 오류 = ({nSwipeRow}, {nSwipeCol})");

            if(_board.IsSwipeable(nSwipeRow, nSwipeCol))
            {
                Block targetBlock = blocks[nSwipeRow, nSwipeCol];
                Block baseBlock = blocks[nRow, nCol];
                Debug.Assert(baseBlock != null && targetBlock != null);

                // 스와이프 대상(targetBlock)과 스와이프 시작점(baseBlock)의 좌표 저장
                Vector3 basePos = baseBlock.blockObj.transform.position;
                Vector3 targetPos = targetBlock.blockObj.transform.position;

                if(targetBlock.IsSwipeable(baseBlock))
                {
                    // baseBlock과 targetBlock의 위치 변경
                    baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                    targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                    yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                    // 스와이프 대상 블럭과 시작 블럭의 블럭 인덱스 교체
                    // nRow, nCol : 시작 블럭의 인덱스
                    // nSwipeRow, nSwipeCol : 대상 블럭의 인덱스
                    blocks[nRow, nCol] = targetBlock;
                    blocks[nSwipeRow, nSwipeCol] = baseBlock;

                    actionResult.value = true;      // 스와이프 실행 결과 반환
                }
            }

            yield break;
        }

        // 유효한 스와이프 액션인지 체크
        public bool IsValideSwipe(int nRow, int nCol, _eSwipe swipeDir)
        {
            switch(swipeDir)
            {
                case _eSwipe.DOWN:
                    return nRow > 0;            // 터치한 블럭이 밑에서 첫번째 행이 아니면 유효
                case _eSwipe.UP:
                    return nRow < _Row - 1;     // 터치한 블럭이 위에서 첫번째 행이 아니면 유효
                case _eSwipe.LEFT:
                    return nCol > 0;            // 터치한 블럭이 가장 왼쪽 열이 아니면 유효
                case _eSwipe.RIGHT:
                    return nCol < _Col - 1;     // 터치한 블럭이 가장 오른쪽 열이 아니면 유효
                default:
                    return false;
            }
        }

        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            yield return _board.Evaluate(matchResult);
        }
    }
}

