using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;     // Block, Vector2Int로 구성된 KVP를 BlockVectorKV로 재정의
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
                    Block block = _board.blocks[nRow, nCol];

                    if(block == null)
                    {
                        continue;
                    }

                    if(_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        // ToDo : 셔플 로직 추가 예정
                    }
                }
            }
        }

        void PrepareShuffleBlocks()
        {

        }

        void RunShuffle(bool animation)
        {

        }
    }
}

