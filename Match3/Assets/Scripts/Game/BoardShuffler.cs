using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;     // Block, Vector2Int로 구성된 KVP를 BlockVectorKV로 타입 재정의
    public class BoardShuffler
    {
        Board _board;
        bool _loadingMode;  // 씬이 시작된 후 보드 구성시 호출되면 true, 플레이 도중에 호출되면 false

        SortedList<int, BlockVectorKV> _orgBlocks = new SortedList<int, BlockVectorKV>();   // 블럭을 섞기 위해 사용하는 SortedList, 리스트에 담으면서 1차로 블럭 셔플
        IEnumerator<KeyValuePair<int, BlockVectorKV>> _it;                                  // SortedList에서 블럭을 하나씩 꺼내는데 사용
        Queue<BlockVectorKV> _UnusedBlocks = new Queue<BlockVectorKV>();                    // 블럭 배치 과정 중에 3매치 발생한 블럭
        bool _listComplete;                                                                 // SortedList에서 조회할 블럭이 남아 있으면 true, 큐에 남은 블럭을 처리하는 중이면 false

        public BoardShuffler(Board board, bool loadingMode)
        {
            _board = board;
            _loadingMode = loadingMode;
        }

        public void Shuffle(bool animation = false)
        {
            PrepareDuplicationData();
            PrepareShuffleBlocks();
            RunShuffle(animation);
        }

        void PrepareDuplicationData()
        {
            for(int nRow = 0; nRow < _board._Row; nRow++)
            {
                for (int nCol = 0; nCol < _board._Col; nCol++)
                {
                    Block block = _board.blocks[nRow, nCol];        // 기준 블럭 설정

                    if(block == null)
                    {
                        continue;
                    }
                   
                    if(_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        block.ResetDuplicationInfo();               // 셔플 가능한 셀이면 중복 갯수를 0으로 리셋하는 메서드
                    }
                    // 셔플할 수 없는 셀 검사, NORMAL 타입이 아닌 경우
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;

                        // (nRow, nCol - 1)에 위치한 블럭과 (nRow, nCol)에 위치한 블럭 종류 비교, 가로 비교
                        if (nCol > 0 && !_board.CanShuffle(nRow, nCol - 1, _loadingMode) && _board.blocks[nRow, nCol - 1].IsSafeEqual(block))
                        {
                            block.horzDuplicate = 2;                            // 가로 중복 카운트를 1에서 2로 증가
                            _board.blocks[nRow, nCol - 1].horzDuplicate = 2;    // 가로 중복 카운트를 1에서 2로 증가
                        }

                        // 세로 비교
                        if (nRow > 0 && !_board.CanShuffle(nRow - 1, nCol, _loadingMode) && _board.blocks[nRow - 1, nCol].IsSafeEqual(block))
                        {
                            block.vertDuplicate = 2;                            // 세로 중복 카운트를 1에서 2로 증가
                            _board.blocks[nRow - 1, nCol].vertDuplicate = 2;    // 세로 중복 카운트를 1에서 2로 증가
                        }
                    }
                }
            }
        }

        void PrepareShuffleBlocks()
        {
            for (int nRow = 0; nRow < _board._Row; nRow++)
            {
                for (int nCol = 0; nCol < _board._Col; nCol++)
                {
                    // 셔플할 수 없는 셀이면 실행하지 않음 
                    if(!_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        continue;
                    }

                    while(true)
                    {
                        // 랜덤값으로 키 생성하고 이미 존재하는 키값인지 체크
                        int random = Random.Range(0, 10000);
                        if(_orgBlocks.ContainsKey(random))
                        {
                            continue;
                        }

                        _orgBlocks.Add(random, new BlockVectorKV(_board.blocks[nRow, nCol], new Vector2Int(nCol, nRow)));
                        break;
                    }
                }
            }
            _it = _orgBlocks.GetEnumerator();
        }

        void RunShuffle(bool animation)
        {
            for (int nRow = 0; nRow < _board._Row; nRow++)
            {
                for (int nCol = 0; nCol < _board._Col; nCol++)
                {
                    // 셔플할 수 없는 셀이면 실행하지 않음 
                    if (!_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        continue;
                    }

                    _board.blocks[nRow, nCol] = GetShuffleBlock(nRow, nCol);
                }
            }
        }

        Block GetShuffleBlock(int nRow, int nCol)
        {
            _eBlockBreed prevBreed = _eBlockBreed.NONE;
            Block firstBlock = null;

            bool useQueue = true;
            while(true)
            {
                BlockVectorKV blockInfo = NextBlock(useQueue);
                Block block = blockInfo.Key;

                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                Debug.Assert(block != null, $"Block Can't Be Null : queue count -> ${_UnusedBlocks.Count}");

                if(prevBreed == _eBlockBreed.NONE)
                {
                    prevBreed = block.breed;
                }

                if(_listComplete)
                {
                    if(firstBlock == null)
                    {
                        firstBlock = block;
                    }
                    else if(ReferenceEquals(firstBlock, block))
                    {
                        _board.ChangeBlock(block, prevBreed);
                    }
                }

                Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

                if(vtDup.x > 2 || vtDup.y > 2)
                {
                    _UnusedBlocks.Enqueue(blockInfo);
                    useQueue = _listComplete || useQueue;

                    continue;
                }

                block.horzDuplicate = vtDup.x;
                block.vertDuplicate = vtDup.y;
                if (block.blockObj != null)
                {
                    //float initx = _board.calcinitx(constants.block_org);
                    //float inity = _board.calcinity(constants.block_org);
                    block.Move(-4.5f + nRow, 4.5f - nCol);
                }

                return block;
            }
        }

        BlockVectorKV NextBlock(bool useQueue)      // useQueue가 true면 큐에서 블럭을 꺼내고 false면 리스트에서 블럭을 꺼내서 리턴
        {
            if(useQueue && _UnusedBlocks.Count > 0)
            {
                return _UnusedBlocks.Dequeue();
            }

            if(_listComplete && _it.MoveNext())
            {
                return _it.Current.Value;
            }

            _listComplete = true;                               // 더 이상 리스트에서 꺼낼 블럭이 없으면 값을 true로 변경

            return new BlockVectorKV(null, Vector2Int.zero);    // 리스트에 블럭이 없으면 빈 블럭(null)을 반환
        }

        Vector2Int CalcDuplications(int nRow, int nCol, Block block)        // block : 새로 배치할 블럭
        {
            int colDup = 1, rowDup = 1;

            if (nCol > 0 && _board.blocks[nRow, nCol - 1].IsSafeEqual(block))
            {
                colDup += _board.blocks[nRow, nCol - 1].horzDuplicate;
            }

            if (nRow > 0 && _board.blocks[nRow - 1, nCol].IsSafeEqual(block))
            {
                rowDup += _board.blocks[nRow - 1, nCol].vertDuplicate;
            }

            if(nCol < _board._Col - 1 && _board.blocks[nRow, nCol + 1].IsSafeEqual(block))
            {
                Block rightBlock = _board.blocks[nRow, nCol + 1];
                colDup += rightBlock.horzDuplicate;

                if(rightBlock.horzDuplicate == 1)
                {
                    rightBlock.horzDuplicate = 2;
                }
            }

            if (nRow < _board._Row - 1 && _board.blocks[nRow + 1, nCol].IsSafeEqual(block))
            {
                Block upperBlock = _board.blocks[nRow + 1, nCol];
                rowDup += upperBlock.vertDuplicate;

                if (upperBlock.vertDuplicate == 1)
                {
                    upperBlock.vertDuplicate = 2;
                }
            }

            return new Vector2Int(colDup, rowDup);
        }
    }
}

