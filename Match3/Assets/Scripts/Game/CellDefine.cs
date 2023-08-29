using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public enum _eTileType
    {
        EMPTY = 0,
        GRASS,      // 여기는 블럭을 배치하지 않음, 사실상 EMPTY
        NORMAL,     // 여기 있는 블럭은 한 번만에 깰 수 있음
        HARD,       // 여기 있는 블럭은 깰 수 없음

        MAX
    }

    static class CellTypeMethod
    {
        // 블럭이 위치할 수 있는 타입인지 체크
        public static bool IsBlockAllocatableType(this _eTileType type)
        {
            return (type == _eTileType.NORMAL);
        }

        // 블럭이 이동 가능한 타입인지 체크
        public static bool IsBlockMovableType(this _eTileType type)
        {
            return (type == _eTileType.NORMAL);
        }
    }

}
