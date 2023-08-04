using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _eBlockType
{
    EMPTY = 0,
    BASIC,

    MAX
}

namespace Match3.Board
{
    public class Block
    {
        protected _eBlockType _blockType;
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
    }
}

