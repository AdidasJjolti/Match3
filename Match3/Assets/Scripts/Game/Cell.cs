using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class Cell
    {
        protected _eTileType _tileType;
        public _eTileType type
        {
            get
            {
                return _tileType;
            }

            set
            {
                _tileType = value;
            }
        }

        public Cell(_eTileType tileType)
        {
            _tileType = tileType;
        }
    }
}

