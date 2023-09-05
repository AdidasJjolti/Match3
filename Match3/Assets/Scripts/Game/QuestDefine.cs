using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public enum _eMatchType
    {
        NONE = 0,

        THREE = 3,          // 3매치
        FOUR,               // 4매치, CLEAR_HORZ 또는 CLEAR_VERT
        FIVE,               // 5매치, CLEAR_LAZER
        THREE_THREE = 6,    // 3 + 3 매치, CLEAR_CIRCLE
        THREE_FOUR,         // 3 + 4 매치, CLEAR_CIRCLE
        THREE_FIVE,         // 3 + 5 매치 ,CLEAR_LAZER
        FOUR_FIVE,          // 4 + 5 매치, CLEAR_LAZER
        FOUR_FOUR = 10,     // 4 + 4 매치, CLEAR_CIRCLE

        MAX
    }

    static class MatchTypeMethod
    {
        public static short ToValue(this _eMatchType matchType)
        {
            return (short)matchType;
        }

        public static _eMatchType Add(this _eMatchType matchTypeSrc, _eMatchType matchTypeTarget)
        {
            if (matchTypeSrc == _eMatchType.FOUR && matchTypeTarget == _eMatchType.FOUR)
            {
                return _eMatchType.FOUR_FOUR;
            }

            return (_eMatchType)((int)matchTypeSrc + (int)matchTypeTarget);
        }
    }
}
