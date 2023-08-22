using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
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

    static class BlockMethod
    {
        // block이 null이 아니고 타겟 블럭과 같은 breed인 경우 true 리턴
        public static bool IsSafeEqual(this Block block, Block targetBlock)
        {
            if (block == null)
            {
                return false;
            }

            return block.IsEqual(targetBlock);
        }
    }
}
