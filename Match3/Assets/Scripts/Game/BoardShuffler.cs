using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;     // Block, Vector2Int�� ������ KVP�� BlockVectorKV�� Ÿ�� ������
    public class BoardShuffler
    {
        Board _board;
        bool _loadingMode;  // ���� ���۵� �� ���� ������ ȣ��Ǹ� true, �÷��� ���߿� ȣ��Ǹ� false

        SortedList<int, BlockVectorKV> _orgBlocks = new SortedList<int, BlockVectorKV>();   // ���� ���� ���� ����ϴ� SortedList, ����Ʈ�� �����鼭 1���� �� ����
        IEnumerator<KeyValuePair<int, BlockVectorKV>> _it;                                  // SortedList���� ���� �ϳ��� �����µ� ���
        Queue<BlockVectorKV> _UnusedBlocks = new Queue<BlockVectorKV>();                    // �� ��ġ ���� �߿� 3��ġ �߻��� ��
        bool _listComplete;                                                                 // SortedList���� ��ȸ�� ���� ���� ������ true, ť�� ���� ���� ó���ϴ� ���̸� false

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

                    // ToDo : ���� ��� �� üũ�ϴ� ���� �ڵ� �ؼ�
                    if(_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        block.ResetDuplicationInfo();
                    }
                    // ������ �� ���� �� �˻�
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;

                        // (nRow, nCol - 1)�� ��ġ�� ���� (nRow, nCol)�� ��ġ�� �� ���� ��
                        if (nCol > 0 && !_board.CanShuffle(nRow, nCol - 1, _loadingMode) && _board.blocks[nRow, nCol - 1].IsSafeEqual(block))
                        {
                            block.horzDuplicate = 2;
                            _board.blocks[nRow, nCol - 1].horzDuplicate = 2;
                        }

                        if (nRow > 0 && !_board.CanShuffle(nRow - 1, nCol, _loadingMode) && _board.blocks[nRow - 1, nCol].IsSafeEqual(block))
                        {
                            block.vertDuplicate = 2;
                            _board.blocks[nRow - 1, nCol].vertDuplicate = 2;
                        }
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

