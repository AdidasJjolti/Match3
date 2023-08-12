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
                _blockBehaviour?.UpdateView(true);      // breed 값이 set되는 경우 블럭 스프라이트 변경
            }
        }
    }
}

