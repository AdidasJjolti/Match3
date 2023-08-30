using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;     // Block, Vector2Int�� ������ KVP�� BlockVectorKV�� using�� ����Ͽ� Ÿ�� ������
    public class BoardShuffler
    {
        Board _board;
        bool _loadingMode;  // ���� ���۵� �� ���� ������ ȣ��Ǹ� true, �÷��� ���߿� ȣ��Ǹ� false

        SortedList<int, BlockVectorKV> _orgBlocks = new SortedList<int, BlockVectorKV>();   // ���� ���� ���� ����ϴ� SortedList, ����Ʈ�� �����鼭 1���� �� ����
        IEnumerator<KeyValuePair<int, BlockVectorKV>> _it;                                  // SortedList���� ���� �ϳ��� �����µ� ����ϴ� Enumerator
        Queue<BlockVectorKV> _UnusedBlocks = new Queue<BlockVectorKV>();                    // �� ��ġ ���� �߿� 3��ġ �߻��� ���� �ӽ� �����ϴ� ť
        bool _listComplete;                                                                 // SortedList���� ��ȸ�� ���� ���� ������ true, ť�� ���� ���� ó���ϴ� ���̸� false

        public BoardShuffler(Board board, bool loadingMode)
        {
            _board = board;
            _loadingMode = loadingMode;
        }

        public void Shuffle(bool animation = false)
        {
            PrepareDuplicationData();               // ���� ���� ���� �� ���� ��Ī ������ ������Ʈ
            PrepareShuffleBlocks();                 // ���� ��� ���� ����Ʈ�� ����
            RunShuffle(animation);                  // ������ �غ��� �����ͷ� ���� ����

            Debug.Log("���� �Ϸ�");
        }

        void PrepareDuplicationData()
        {
            for(int nRow = 0; nRow < _board._Row; nRow++)
            {
                for (int nCol = 0; nCol < _board._Col; nCol++)
                {
                    Block block = _board.blocks[nRow, nCol];        // ���� �� ����

                    if(block == null)
                    {
                        continue;
                    }
                   
                    if(_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        block.ResetDuplicationInfo();               // ���� ������ ���̸� �ߺ� ������ 0���� �����ϴ� �޼���
                    }
                    // ������ �� ���� �� �˻�, NORMAL Ÿ���� �ƴ� ���
                    // ���� ���� � ������ �Ͻ������� �̵����� ���ϴ� �� ������� �˻�
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;

                        // (nRow, nCol - 1)�� ��ġ�� ���� (nRow, nCol)�� ��ġ�� ���� ��� ������ �� ���� ���̰� ���� �� �������� ��, ���� ��
                        if (nCol > 0 && !_board.CanShuffle(nRow, nCol - 1, _loadingMode) && _board.blocks[nRow, nCol - 1].IsSafeEqual(block))
                        {
                            block.horzDuplicate = 2;                            // ���� �ߺ� ī��Ʈ�� 1���� 2�� ����
                            _board.blocks[nRow, nCol - 1].horzDuplicate = 2;    // ���� �ߺ� ī��Ʈ�� 1���� 2�� ����
                        }

                        // ���� ��
                        if (nRow > 0 && !_board.CanShuffle(nRow - 1, nCol, _loadingMode) && _board.blocks[nRow - 1, nCol].IsSafeEqual(block))
                        {
                            block.vertDuplicate = 2;                            // ���� �ߺ� ī��Ʈ�� 1���� 2�� ����
                            _board.blocks[nRow - 1, nCol].vertDuplicate = 2;    // ���� �ߺ� ī��Ʈ�� 1���� 2�� ����
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
                    // ������ �� ���� ���̸� �������� ����, ����Ʈ�� �������� ����
                    if(!_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        continue;
                    }

                    while(true)
                    {
                        // ���������� Ű �����ϰ� �̹� �����ϴ� Ű������ üũ
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

        // ��ü �� ����
        void RunShuffle(bool animation)
        {
            for (int nRow = 0; nRow < _board._Row; nRow++)
            {
                for (int nCol = 0; nCol < _board._Col; nCol++)
                {
                    // ������ �� ���� ���̸� �������� ���� 
                    if (!_board.CanShuffle(nRow, nCol, _loadingMode))
                    {
                        continue;
                    }

                    _board.blocks[nRow, nCol] = GetShuffleBlock(nRow, nCol);        // ���� ��� ���� ���� ��ġ�� ���� �޾� �ͼ� ����
                }
            }
        }

        // row, col ��ǥ�� ��ġ�� �� �ִ� ���� ������ ����
        Block GetShuffleBlock(int nRow, int nCol)
        {
            _eBlockBreed prevBreed = _eBlockBreed.NONE;
            Block firstBlock = null;            // ����Ʈ�� ���� ó���ϰ� UnusedBlocks ť���� ���� �ִ� ��� �ߺ� üũ�� ���� ���, ť���� ���� ù��° ��

            //int i = 0;
            bool useQueue = true;               // _UnusedBlocks���� �������� true, _it���� �������� false
            while(true)    //  || i < 1000
            {
                // _UnusedBlocks ť���� ��� ������, ù��° �ĺ�
                BlockVectorKV blockInfo = NextBlock(useQueue);
                Block block = blockInfo.Key;

                // ť���� ���� ����� ���� ��� �ٽ� �ѹ� �� ť���� ����� ã��
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                Debug.Assert(block != null, $"Block Can't Be Null : queue count -> {_UnusedBlocks.Count}");     // ���� null�̸� �α� ��� (ť�� ��� ����)

                if(prevBreed == _eBlockBreed.NONE)
                {
                    prevBreed = block.breed;
                }

                // ����Ʈ�� ��� ó���� ���
                if(_listComplete)
                {
                    if(firstBlock == null)
                    {
                        firstBlock = block;         // ť���� ���� ù��° ��
                    }
                    else if(ReferenceEquals(firstBlock, block))
                    {
                        _board.ChangeBlock(block, prevBreed);
                    }
                }

                Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

                //i++;

                if (vtDup.x > 2 || vtDup.y > 2)
                {
                    _UnusedBlocks.Enqueue(blockInfo);
                    useQueue = _listComplete || !useQueue;

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

            //Debug.Log(i.ToString());
            //return null;
        }

        BlockVectorKV NextBlock(bool useQueue)      // useQueue�� true�� ť���� ���� ������ false�� ����Ʈ���� ���� ������ ����
        {
            if(useQueue && _UnusedBlocks.Count > 0)
            {
                return _UnusedBlocks.Dequeue();
            }

            if(!_listComplete && _it.MoveNext())
            {
                return _it.Current.Value;
            }

            _listComplete = true;                               // �� �̻� ����Ʈ���� ���� ���� ������ ���� true�� ����

            return new BlockVectorKV(null, Vector2Int.zero);    // ����Ʈ�� ���� ������ �� ��(null)�� ��ȯ
        }

        Vector2Int CalcDuplications(int nRow, int nCol, Block block)        // block : ���� ��ġ�� ��
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

