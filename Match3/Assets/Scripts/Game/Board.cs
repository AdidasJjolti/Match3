using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;
using Util;

namespace Match3.Board
{
    public class Board
    {
        int _row;
        int _col;
        Transform _container;

        BoardEnumerator _enumerator;

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

            _enumerator = new BoardEnumerator(this);
        }

        internal void ComposeStage(Transform container = null)
        {
            _container = container;
            
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

        public bool IsSwipeable(int nRow, int nCol)
        {
            return _cells[nRow, nCol].type.IsBlockMovableType();    // 움직일 수 있는 블럭인 경우 스와이프 가능하므로 true 반환
        }

        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            bool matchedBlockFound = UpdateAllBlocksMathcedStatus();        // 3매치 블럭이 있으면 true 반환

            if(matchedBlockFound == false)
            {
                matchResult.value = false;
                yield break;
            }

            List<Block> clearBlocks = new List<Block>();

            for(int nRow = 0; nRow < _Row; nRow++)
            {
                for(int nCol = 0; nCol < _Col; nCol++)
                {
                    Block block = _blocks[nRow, nCol];
                    block?.DoEvaluation(_enumerator, nRow, nCol);

                    if(block != null)
                    {
                        if(block._status == _eBlockStatus.CLEAR)
                        {
                            clearBlocks.Add(block);
                            _blocks[nRow, nCol] = null;
                        }
                    }
                }
            }

            clearBlocks.ForEach((block) => block.Destroy());
            matchResult.value = true;

            yield break;
        }

        // 3매치 블럭이 있는지 체크
        public bool UpdateAllBlocksMathcedStatus()
        {
            List<Block> matchedBlockList = new List<Block>();
            int count = 0;

            for (int nRow = 0; nRow < _Row; nRow++)
            {
                for (int nCol = 0; nCol < _Col; nCol++)
                {
                    if (EvalBlocksIfMatched(nRow, nCol, matchedBlockList))
                    {
                        count++;
                    }
                }
            }

            return count > 0;
        }

        public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
        {
            bool found = false;
            Block baseBlock = _blocks[nRow, nCol];

            if(baseBlock == null)
            {
                return false;
            }

            if(baseBlock._match != _eMatchType.NONE || !baseBlock.IsValidate() || _cells[nRow, nCol].IsObstracle())
            {
                return false;
            }

            matchedBlockList.Add(baseBlock);

            Block block;

            // baseBlock 기준 우측 블럭 검사
            for (int i = nCol + 1; i < nCol; i++)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Add(block);
            }

            // baseBlock 기준 좌측 블럭 검사
            for (int i = nCol - 1; i >= 0; i--)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Insert(0, block);
            }

            if(matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, true);
                found = true;
            }

            matchedBlockList.Clear();

            matchedBlockList.Add(baseBlock);

            // baseBlock 기준 위쪽 블럭 검사
            for (int i = nRow + 1; i < nRow; i++)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Add(block);
            }

            // baseBlock 기준 아래쪽 블럭 검사
            for (int i = nRow - 1; i >= 0; i--)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Insert(0, block);
            }

            if (matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, true);
                found = true;
            }

            matchedBlockList.Clear();

            return found;
        }

        void SetBlockStatusMatched(List<Block> blockList, bool horz)
        {
            int matchCount = blockList.Count;
            blockList.ForEach(block => block.UpdateBlockStatusMatched((_eMatchType)matchCount));
        }
    }
}

