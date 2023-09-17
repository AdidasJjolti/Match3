using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;
using Util;

namespace Match3.Board
{
    using IntIntKV = KeyValuePair<int, int>;
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

        // 3매치 블럭을 판단하여 제거하는 게임 규칙 실행
        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            bool matchedBlockFound = UpdateAllBlocksMathcedStatus();        // 3매치 블럭이 있으면 true 반환

            if(matchedBlockFound == false)
            {
                matchResult.value = false;
                yield break;
            }

            List<Block> clearBlocks = new List<Block>();        // 제거될 블럭을 저장하는 리스트

            for(int nRow = 0; nRow < _Row; nRow++)
            {
                for(int nCol = 0; nCol < _Col; nCol++)
                {
                    Block block = _blocks[nRow, nCol];
                    block?.DoEvaluation(_enumerator, nRow, nCol);       // 3매치가 되는 블럭인지 체크

                    if(block != null)
                    {
                        // 클러어 가능한 블럭이면 제거될 블럭 리스트에 넣고 blocks 배열에서 제거
                        if(block._status == _eBlockStatus.CLEAR)
                        {
                            clearBlocks.Add(block);
                            _blocks[nRow, nCol] = null;
                        }
                    }
                }
            }

            // 리스트에 있는 블럭 모두 제거
            clearBlocks.ForEach((block) => block.Destroy());

            yield return new WaitForSeconds(0.15f);         // 블럭이 제거되는 동안 딜레이 적용

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
                        count++;    // 매치되는 블럭이 생길때마다 count 증가
                    }
                }
            }

            return count > 0;
        }

        // 가로 또는 세로로 블럭 검사하여 매치되는 블럭이 하나라도 있으면 true 반환
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
                SetBlockStatusMatched(matchedBlockList, false);
                found = true;
            }

            matchedBlockList.Clear();

            return found;
        }

        void SetBlockStatusMatched(List<Block> blockList, bool horz)        // 3매치 블럭의 배치가 가로면 true, 세로면 false
        {
            int matchCount = blockList.Count;
            blockList.ForEach(block => block.UpdateBlockStatusMatched((_eMatchType)matchCount));
            
            // 위의 람다식과 동일한 코드
            //foreach(var block in blockList)
            //{
            //    block.UpdateBlockStatusMatched((_eMatchType)matchCount);
            //}
        }

        public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
        {
            Debug.Log("ArrangeBlocksAfterClean 시작");
            SortedList<int, int> tempBlocks = new SortedList<int, int>();       // 빈 블럭으로 남는 위치를 저장할 임시 리스트
            List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();            // 이동이 필요한 블럭 리스트를 저장할 리스트

            for(int nRow = 0; nRow < _Row; nRow++)
            {
                tempBlocks.Clear();

                for(int nCol = 0; nCol < _Col; nCol++)
                {
                    if(CanBlockBeAllocatable(nRow, nCol))
                    {
                        tempBlocks.Add(nCol, nCol);
                    }
                }

                IComparer<int> reverseComparer = new ReverseComparer<int>();
                SortedList<int, int> emptyBlocks = new SortedList<int, int>(tempBlocks, reverseComparer);      // 빈 블럭으로 남는 위치를 저장할 리스트

                if (emptyBlocks.Count == 0)
                {
                    continue;
                }

                // 가장 아래에 비어있는 블럭 처리
                IntIntKV first = emptyBlocks.First();

                // 비어있는 블럭 위쪽 방향으로 이동 가능한 블럭을 탐색하면서 빈 블럭 자리를 채움
                for(int col = first.Value - 1; col >= 0; col--)
                {
                    Block block = _blocks[nRow, col];

                    if(block == null || _cells[nRow, col].type == _eTileType.EMPTY)
                    {
                        continue;
                    }

                    // 이동할 블럭 발견
                    block._dropDistance = new Vector2(0, col - first.Value);
                    movingBlocks.Add(block);

                    Debug.Assert(_cells[first.Value, col].IsObstracle() == false, $"{_cells[first.Value, col]}");
                    // 이동될 위치로 board에서 저장된 위치 이동
                    _blocks[nRow, first.Value] = block;
                    // 다른 곳으로 이동하여 현재 위치 비워둠
                    _blocks[nRow, col] = null;

                    emptyBlocks.RemoveAt(0);
                    emptyBlocks.Add(col, col);

                    first = emptyBlocks.First();
                    col = first.Value;
                }
            }

            yield return null;

            if(emptyRemainBlocks.Count > 0)
            {
                unfilledBlocks.AddRange(emptyRemainBlocks);
            }

            yield break;
        }

        bool CanBlockBeAllocatable(int nRow, int nCol)
        {
            if(!_cells[nRow, nCol].type.IsBlockAllocatableType())
            {
                return false;
            }

            return _blocks[nRow, nCol] == null;
        }
    }
}

