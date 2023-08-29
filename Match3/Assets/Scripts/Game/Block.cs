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
                _blockBehaviour?.UpdateView(true);      // breed 값이 set되는 경우 블럭 스프라이트 변경
            }
        }

        // Block에 연결된 게임 오브젝트의 트랜스폼 반환
        public Transform blockObj
        {
            get
            {
                return transform;
            }
        }

        Vector2Int _vtDuplicate;  // 블럭 중복 갯수, 셔플할 때 중복 검사에 사용

        // 가로 방향 중복 검사에 사용
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

        // 세로 방향 중복 검사에 사용
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

        // 같은 종류의 breed인지 검사
        public bool IsEqual(Block target)
        {
            if(IsMatchableBlock() && this.breed == target.breed)
            {
                return true;
            }

            return false;
        }

        // 3매치 대상이 되는 블럭인지 검사, EMPTY 타입이 아닌 블럭 검사
        public bool IsMatchableBlock()
        {
            return (type == _eBlockType.BASIC);
        }
    }
}

