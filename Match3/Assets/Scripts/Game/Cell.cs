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

        /// <summary>
        /// 셀 타입이 EMPTY인지 체크
        /// </summary>
        /// <returns></returns>
        public bool IsObstracle()
        {
            return _tileType == _eTileType.EMPTY;
        }
    }
}

