using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _eBlockType
{
    EMPTY = 0,
    BASIC,

    MAX
}

public enum _eBlockBreed
{
    NONE = -1,
    BREED_0,
    BREED_1,
    BREED_2,
    BREED_3,
    BREED_4,
    BREED_5,

    MAX
}

namespace Match3.Board
{
    public class Block : MonoBehaviour
    {
        [SerializeField] protected _eBlockType _blockType;
        public _eBlockType type
        {
            get
            {
                return _blockType;
            }

            set
            {
                _blockType = value;
            }
        }

        public Block(_eBlockType blockType)
        {
            _blockType = blockType;
        }

        protected BlockBehaviour _blockBehaviour;
        public BlockBehaviour blockBehaviour
        {
            get
            {
                return _blockBehaviour;
            }
            
            set
            {
                _blockBehaviour = value;
                //_blockBehaviour.SetBlock(this);
            }
        }

        public _eBlockBreed _breed;
        public _eBlockBreed breed
        {
            get
            {
                return _breed;
            }

            set
            {
                _breed = value;
                _blockBehaviour?.UpdateView(true);      // breed ���� set�Ǵ� ��� �� ��������Ʈ ����
            }
        }

        // Block�� ����� ���� ������Ʈ�� Ʈ������ ��ȯ
        public Transform blockObj
        {
            get
            {
                return transform;
            }
        }

        Vector2Int _vtDuplicate;  // �� �ߺ� ����, ������ �� �ߺ� �˻翡 ���

        // ���� ���� �ߺ� �˻翡 ���
        public int horzDuplicate
        {
            get
            {
                return _vtDuplicate.x;
            }
            set
            {
                _vtDuplicate.x = value;
            }
        }

        // ���� ���� �ߺ� �˻翡 ���
        public int vertDuplicate
        {
            get
            {
                return _vtDuplicate.y;
            }
            set
            {
                _vtDuplicate.y = value;
            }
        }

        public void ResetDuplicationInfo()
        {
            _vtDuplicate.x = 0;
            _vtDuplicate.y = 0;
        }

        // ���� ������ breed���� �˻�
        public bool IsEqual(Block target)
        {
            if(IsMatchableBlock() && this.breed == target.breed)
            {
                return true;
            }

            return false;
        }

        // 3��ġ ����� �Ǵ� ������ �˻�, EMPTY Ÿ���� �ƴ� �� �˻�
        public bool IsMatchableBlock()
        {
            return !(type == _eBlockType.EMPTY);
        }

        // block�� null�� �ƴϰ� Ÿ�� ���� ���� breed�� ��� true ����
        public bool IsSafeEqual(Block block, Block targetBlock)
        {
            if(block == null)
            {
                return false;
            }

            return block.IsEqual(targetBlock);
        }
    }
}

