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
            Vector2 pos = new Vector2(point.x + (_Col / 2.0f), point.y * -1 + (_Row / 2.0f));
            int nRow = (int)pos.x;
            int nCol = (int)pos.y;

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

        public IEnumerator CoDoSwipeAction(int nRow, int nCol, _eSwipe swipeDir, Returnable<bool> actionResult)
        {
            actionResult.value = false;     // �ڷ�ƾ ���ϰ� ����

            // ���������Ǵ� ���� ��ġ ���ϱ�
            int nSwipeRow = nRow, nSwipeCol = nCol;
            nSwipeRow += swipeDir.GetTargetCol();
            nSwipeCol += swipeDir.GetTargetRow();

            Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, $"Invalid Swipe : ({nSwipeRow}, {nSwipeCol})");        // �������� �Ǵ� ���� ��ġ�� Ŭ�� ��ġ�� ���� ��� �������� ����
            Debug.Assert(nSwipeRow >= 0 && nSwipeRow < _Row && nSwipeCol >= 0 && nSwipeCol < _Col, $"Swipe Ÿ�� �� �ε��� ���� = ({nSwipeRow}, {nSwipeCol})");

            if(_board.IsSwipeable(nSwipeRow, nSwipeCol))
            {
                Block targetBlock = blocks[nSwipeRow, nSwipeCol];
                Block baseBlock = blocks[nRow, nCol];
                Debug.Assert(baseBlock != null && targetBlock != null);

                // �������� ���(targetBlock)�� �������� ������(baseBlock)�� ��ǥ ����
                Vector3 basePos = baseBlock.blockObj.transform.position;
                Vector3 targetPos = targetBlock.blockObj.transform.position;

                if(targetBlock.IsSwipeable(baseBlock))
                {
                    // baseBlock�� targetBlock�� ��ġ ����
                    baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                    targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                    yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                    // �������� ��� ���� ���� ���� �� �ε��� ��ü
                    // nRow, nCol : ���� ���� �ε���
                    // nSwipeRow, nSwipeCol : ��� ���� �ε���
                    blocks[nRow, nCol] = targetBlock;
                    blocks[nSwipeRow, nSwipeCol] = baseBlock;

                    actionResult.value = true;      // �������� ���� ��� ��ȯ
                }
            }

            yield break;
        }

        // ��ȿ�� �������� �׼����� üũ
        public bool IsValideSwipe(int nRow, int nCol, _eSwipe swipeDir)
        {
            switch(swipeDir)
            {
                case _eSwipe.DOWN:
                    return nRow > 0;            // ��ġ�� ���� �ؿ��� ù��° ���� �ƴϸ� ��ȿ
                case _eSwipe.UP:
                    return nRow < _Row - 1;     // ��ġ�� ���� ������ ù��° ���� �ƴϸ� ��ȿ
                case _eSwipe.LEFT:
                    return nCol > 0;            // ��ġ�� ���� ���� ���� ���� �ƴϸ� ��ȿ
                case _eSwipe.RIGHT:
                    return nCol < _Col - 1;     // ��ġ�� ���� ���� ������ ���� �ƴϸ� ��ȿ
                default:
                    return false;
            }
        }

        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            yield return _board.Evaluate(matchResult);
        }

        public IEnumerator PostprocessAfterEvaluate()
        {
            Debug.Log("PostprocessAfterEvaluate ����");
            List<KeyValuePair<int, int>> unfilledBlocks = new List<KeyValuePair<int, int>>();
            List<Block> movingBlocks = new List<Block>();

            // ���ŵ� ���� ���� �� ���ġ
            yield return _board.ArrangeBlocksAfterClean(unfilledBlocks, movingBlocks);
            // ������ ���� ��� ���̵��� �ٸ� ���� ����ϴ� ���� ���
            yield return WaitForDropping(movingBlocks);
        }

        public IEnumerator WaitForDropping(List<Block> movingBlocks)
        {
            WaitForSeconds waitForSecond = new WaitForSeconds(0.05f);       // 0.05�ʸ��� üũ

            // �ִϸ��̼� ���� ���� ���� ������ �ݺ�
            while(true)
            {
                bool continued = false;

                for(int i = 0; i < movingBlocks.Count; i++)
                {
                    if(movingBlocks[i]._isMoving)
                    {
                        continued = true;
                        break;
                    }
                }

                // �����̴� ���� ������ while�� ����
                if(!continued)
                {
                    break;
                }

                yield return waitForSecond;
            }

            movingBlocks.Clear();   // ����Ʈ �ʱ�ȭ
            yield break;
        }
    }
}

