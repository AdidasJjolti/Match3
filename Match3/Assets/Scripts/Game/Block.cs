using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

        internal void Move(float x, float y)
        {
            this.transform.position = new Vector3(x, y);
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
            return (type == _eBlockType.BASIC);
        }
    }
}

