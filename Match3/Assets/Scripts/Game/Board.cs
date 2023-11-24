using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;
using Util;
using Match3.Stage;

namespace Match3.Board
{
    using IntIntKV = KeyValuePair<int, int>;
    public class Board
    {
        int _row;
        int _col;
        Transform _container;
        G_TileMap2D _tileMap2D;

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

        internal void ComposeStage(G_TileMap2D tilemap2D, Transform container = null)
        {
            _tileMap2D = tilemap2D;
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
            return _Row / 2.0f - offset;
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

            // 새로운 breed를 설정하고 스프라이트를 결정된 breed에 맞게 변경
            block.breed = genBreed;
            block.blockBehaviour.ChangeView(genBreed);
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
                        // 클리어 가능한 블럭이면 제거될 블럭 리스트에 넣고 blocks 배열에서 제거
                        if(block._status == _eBlockStatus.CLEAR)
                        {
                            clearBlocks.Add(block);
                            _blocks[nRow, nCol] = null;
                        }
                    }
                }
            }

            // 1. clearBlocks 리스트의 크기 판단
            // 2. 3이면 기존 로직 그대로 실행, 4이면 레이저 아이템 생성, 5 이상이면 폭탄 생성
            // 3. clearBlocks 리스트의 크기가 4 또는 5인 경우 리스트 내 블럭을 모두 제거하기 전에 하나를 무작위로 뽑아 clearBlocks에서 제거
            // 4. 리스트에서 제거한 블록을 아이템 블록으로 모습 변경, 속성 변경
            // 5. 이후에는 clearBlocks 리스트 내 남은 블럭을 모두 제거하는 코드 실행

            // 문제 1 : 리스트에서 빠진 블록의 스프라이트가 변경되지 않음
            // 해결 방안 : 스프라이트 변경 로직을 보강하여 아이템으로 바꾸도록 함
            // 해결 완료

            // 문제 2 : 리스트에서 빠진 블록이 제자리에서 머물러있기만하고 빈 자리로 드랍하지 않음
            // 해결 방안 : ArrangeBlocksAfterClean에서 해당 블록을 인지하고 있는지 확인 필요
            // 해결 완료 : clearBlocks로 add될 때 이미 block이 null 처리된 것을 아이템 블록으로 변경된 경우 _blocks 배열에 다시 채워넣음

            // 문제 3 : 서로 다른 두 종류의 블록이 각각 3개, 4개 제거되는 경우 아이템 블록을 4개 제거되는 블록 중에서 선택해야 함
            // 해결 방안 : 서로 다른 두 종류의 블록이 제거되는 경우 _eMatchType이 THREE가 아닌 블록이 있는지 체크하여 해당 블록들 중에서만 무작위로 1개를 골라 아이템 생성 로직 실행

            int listSize = clearBlocks.Count;

            // 이미 제거할 블록에 아이템이 포함된 경우 체크
            bool containItem = false;
            for(int i = 0; i < clearBlocks.Count; i++)
            {
                if(clearBlocks[i].type == _eBlockType.ITEM)
                {
                    containItem = true;
                    break;
                }
            }

            // 이미 제거할 블록 리스트에 아이템이 포함된 경우 다시 아이템을 생성하는 로직을 실행하지 않음
            if(!containItem)
            {
                bool isSameBlock = true;

                // 아이템 블록 생성 조건 1
                // 모두 같은 종류의 블록이고 clearBlocks의 크기가 3보다 크면 아이템 블록 생성 코드 실행
                for (int i = 1; i < listSize; i++)
                {
                    if (clearBlocks[0].breed != clearBlocks[i].breed)
                    {
                        isSameBlock = false;
                        break;
                    }
                }

                if (listSize > 3 && isSameBlock)
                {
                    // 리스트에서 임의의 블록을 선정하여 블록 매치 갯수에 맞는 아이템으로 변경
                    int num = Random.Range(0, listSize);
                    clearBlocks[num].type = _eBlockType.ITEM;
                    clearBlocks[num].blockBehaviour.UpdateView(clearBlocks);
                    clearBlocks[num].durability = 1;
                    clearBlocks[num]._match = _eMatchType.NONE;
                    clearBlocks[num]._questType = _eBlockQuestType.CLEAR_SIMPLE;
                    clearBlocks[num]._status = _eBlockStatus.NORMAL;

                    // 생성된 아이템 블록을 _blocks 배열에 다시 입력
                    float row = clearBlocks[num].blockObj.localPosition.x + 4.5f;
                    float col = 4.5f - clearBlocks[num].blockObj.localPosition.y;
                    _blocks[(int)row, (int)col] = clearBlocks[num].GetComponent<Block>();

                    clearBlocks.RemoveAt(num);
                }

                // 아이템 생성 조건 2
                // 서로 다른 두 종류의 블록이면 _eMatchType 검사하여 THREE가 아닌 블록이 있는지 확인
                // _eMatchType이 THREE가 아닌 블록이 있다면 해당 블록들 중에서 아이템 블록을 생성

                List<int> reserveBlocks = new List<int>();  // _eMatchType이 THREE가 아닌 블록의 위치를 저장할 리스트
                List<Block> resBlocks = new List<Block>();  // _eMatchType이 THREE가 아닌 블록을 저장할 리스트
                int idx = 0;                                // 아이템 블록을 생성할 clearBlocks의 인덱스 저장

                if (!isSameBlock)
                {
                    for (int i = 0; i < clearBlocks.Count; i++)
                    {
                        if (clearBlocks[i]._match == _eMatchType.FOUR || clearBlocks[i]._match == _eMatchType.FIVE)
                        {
                            reserveBlocks.Add(i);
                            resBlocks.Add(clearBlocks[i]);
                        }
                    }

                    if (reserveBlocks.Count > 0)
                    {
                        idx = reserveBlocks[Random.Range(0, reserveBlocks.Count)];  // _eMatchType이 THREE가 아닌 블록 중 1개를 뽑은 clearBlocks 리스트 내 인덱스

                        clearBlocks[idx].type = _eBlockType.ITEM;
                        clearBlocks[idx].blockBehaviour.UpdateView(resBlocks);
                        clearBlocks[idx].durability = 1;
                        clearBlocks[idx]._match = _eMatchType.NONE;
                        clearBlocks[idx]._questType = _eBlockQuestType.CLEAR_SIMPLE;
                        clearBlocks[idx]._status = _eBlockStatus.NORMAL;

                        // 생성된 아이템 블록을 _blocks 배열에 다시 입력
                        float row = clearBlocks[idx].blockObj.localPosition.x + 4.5f;
                        float col = 4.5f - clearBlocks[idx].blockObj.localPosition.y;
                        _blocks[(int)row, (int)col] = clearBlocks[idx].GetComponent<Block>();

                        clearBlocks.RemoveAt(idx);
                    }
                }
            }

            GameManager.Instance.ReduceRemainingBlocks(clearBlocks.Count);

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

            // baseBlock 기준 아래쪽 블럭 검사
            for (int i = nCol + 1; i < _Col; i++)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}에 위치한 블록 기준 아래쪽 블록과 일치");
                matchedBlockList.Add(block);
            }

            // baseBlock 기준 위쪽 블럭 검사
            for (int i = nCol - 1; i >= 0; i--)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}에 위치한 블록 기준 위쪽 블록과 일치");
                matchedBlockList.Insert(0, block);
            }

            if(matchedBlockList.Count >= 3 || (matchedBlockList.Count == 2 && matchedBlockList[0].breed == _eBlockBreed.BOMB))
            {
                foreach (var item in matchedBlockList)
                {
                    Debug.Log($"{item.blockObj.localPosition.x}, {item.blockObj.localPosition.y}에 위치한 블록이 리스트에 추가됨");
                }

                SetBlockStatusMatched(matchedBlockList, false);  // 세로 매치 발생
                found = true;
            }

            matchedBlockList.Clear();

            matchedBlockList.Add(baseBlock);

            // baseBlock 기준 오른쪽 블럭 검사
            for (int i = nRow + 1; i < _Row; i++)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}에 위치한 블록 기준 오른쪽 블록과 일치");
                matchedBlockList.Add(block);
            }

            // baseBlock 기준 왼쪽 블럭 검사
            for (int i = nRow - 1; i >= 0; i--)
            {
                block = _blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                //Debug.Log($"{baseBlock.transform.localPosition.x}, {baseBlock.transform.localPosition.y}에 위치한 블록 기준 왼쪽 블록과 일치");
                matchedBlockList.Insert(0, block);
            }

            if (matchedBlockList.Count >= 3 || (matchedBlockList.Count == 2 && matchedBlockList[0].breed > _eBlockBreed.ITEM && matchedBlockList[0].breed < _eBlockBreed.ITEM_MAX))
            {
                foreach (var item in matchedBlockList)
                {
                    Debug.Log($"{item.blockObj.localPosition.x}, {item.blockObj.localPosition.y}에 위치한 블록이 리스트에 추가됨");
                }

                SetBlockStatusMatched(matchedBlockList, true);  // 가로 매치 발생
                found = true;
            }

            matchedBlockList.Clear();

            return found;
        }

        void SetBlockStatusMatched(List<Block> blockList, bool horz)        // 3매치 블럭의 배치가 가로면 true, 세로면 false
        {
            int matchCount = blockList.Count;

            if(matchCount == 2 && blockList[0].breed == _eBlockBreed.BOMB)
            {
                matchCount = 3;
            }

            blockList.ForEach(block => block.UpdateBlockStatusMatched((_eMatchType)matchCount));
            
            foreach(var block in blockList)
            {
                switch (block._match)
                {
                    case _eMatchType.THREE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 3개 블록 매칭됨");
                        break;
                    case _eMatchType.FOUR:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 4개 블록 매칭됨");
                        break;
                    case _eMatchType.FIVE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 5개 블록 매칭됨");
                        break;
                    case _eMatchType.THREE_THREE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 3+3개 블록 매칭됨");
                        break;
                    case _eMatchType.THREE_FOUR:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 3+4개 블록 매칭됨");
                        break;
                    case _eMatchType.THREE_FIVE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 3+5개 블록 매칭됨");
                        break;
                    case _eMatchType.FOUR_FIVE:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 4+5개 블록 매칭됨");
                        break;
                    case _eMatchType.FOUR_FOUR:
                        Debug.Log($"{block.blockObj.localPosition.x}, {block.blockObj.localPosition.y}에 위치한 블록의 스테이터스 : 4+4개 블록 매칭됨");
                        break;
                }
            }

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

                    if(block == null || _cells[nRow, col].type != _eTileType.NORMAL)
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

        public IEnumerator SpawnBlocksAfterClean(List<Block> movingBlocks)
        {
            Debug.Log("SpawnBlocksAfterClean 시작");
            for (int row = 0; row < _Row; row++)
            {
                for(int col = _Col - 1; col >= 0; col--)
                {
                    if(_blocks[row, col] == null)   // 해당 위치에 블럭이 삭제된 경우 아래 코드 실행
                    {
                        //int topCol = _Col;
                        // 블럭이 생성되는 원점 설정, 0은 보드 상단 기준으로 첫번째 블럭이 생성되는 위치
                        // 새 블럭이 생성되면 1씩 증가해서 다음 블럭이 이전 블럭의 위쪽에서 드롭이 시작하도록 함
                        int spawnBaseY = 0;

                        for(int y = col; y >= 0; y--)   // y는 블럭이 도착할 좌표
                        {
                            if(_blocks[row, y] != null || !CanBlockBeAllocatable(row, y))
                            {
                                continue;
                            }

                            Block block = SpawnBlockWithDrop(row, y, row, spawnBaseY);
                            if(block != null)
                            {
                                movingBlocks.Add(block);
                            }

                            //spawnBaseY--;
                        }

                        break;
                    }
                }
            }

            yield return null;
        }

        // row, col : 블럭 생성 후 저장되는(도착하는) 위치
        // spawnedRow, spawnedCol : 블럭이 생성되는 위치, row, col까지 드롭
        Block SpawnBlockWithDrop(int row, int col, int spawnedRow, int spawnedCol)
        {
            float initX = CalcInitX(Core.Constants.BLOCK_ORG);
            float initY = CalcInitY(Core.Constants.BLOCK_ORG);

            //Block block = _stageBuilder.SpawnBlock().InstantiateBlockObj(_blockPrefab, _container);
            Block block = _tileMap2D.RespawnBlock(row, col);
            if (block != null)
            {
                _blocks[row, col] = block;
                block.Move(initX + (float)spawnedRow, initY + (float)spawnedCol);
                block._dropDistance = new Vector2(0, spawnedCol - col);
            }

            return block;
        }
    }
}

