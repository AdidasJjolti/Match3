using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public enum _eBlockType
    {
        EMPTY = 0,
        BASIC,
        ITEM,
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
        MAX,

        ITEM = 10,
        HORIZONTAL,
        VERTICAL,
        BOMB,
        ITEM_MAX
    }

    public enum _eBlockStatus
    {
        NONE = -1,
        NORMAL,     // 기본 상태
        MATCH,      // 매칭 블럭이 있는 상태
        CLEAR,      // 클리어 예정 상태

        MAX
    }

    public enum _eBlockQuestType
    {
        NONE = -1,
        CLEAR_SIMPLE,           // 단일 블럭 제거
        CLEAR_HORZ,             // 세로줄 블럭 제거
        CLEAR_VERT,             // 가로줄 블럭 제거
        CLEAR_CIRCLE,           // 주변 블럭 제거
        CLEAR_LAZER,            // 동일한 종류 블럭 모두 제거
        CLEAR_HORZ_BUFF,        // HORZ, CIRCLE 조합
        CLEAR_VERT_BUFF,        // VERT, CIRCLE 조합
        CLEAR_CIRCLE_BUFF,      // CIRCLE, CIRCLE 조합
        CLEAR_LAZER_BUFF,       // LAZER, LAZER 조합

        MAX
    }

    static class BlockMethod
    {
        /// <summary>
        /// block이 null이 아니고 타겟 블럭과 같은 breed인 경우 true 리턴
        /// </summary>
        /// <param name="block"></param>
        /// <param name="targetBlock"></param>
        /// <returns></returns>
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
