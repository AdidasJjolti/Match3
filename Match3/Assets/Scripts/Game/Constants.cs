using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public static class Constants
    {
        public static float BLOCK_ORG = 0.5f;           // 블럭의 출력 원점
        public static float SWIPE_DURATION = 0.2f;      // 블럭 스와이프 애니메이션 재생 시간
        public static float BLOCK_DESTROY_SCALE = 0.3f; // 블럭 삭제될 때 줄어드는 크기
    }
}
