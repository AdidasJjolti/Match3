using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public enum _eSwipe     //  스와이프 방향 enum
    {
        NONE = -1,
        RIGHT,
        UP,
        LEFT,
        DOWN,

        MAX
    }

    public static class TouchEvaluator
    {
        // 스와이프 방향 구하기
        public static _eSwipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
        {
            float angle = EvalDragAngle(vtStart, vtEnd);        // 스와이프 각도로 방향 판단
            if(angle < 0)
            {
                return _eSwipe.NONE;
            }

            int swipe = (((int)angle + 45) % 360) / 90;

            switch(swipe)
            {
                case 0:
                    return _eSwipe.RIGHT;       // 0 ~ 45, 315 ~ 360 (0)
                case 1:
                    return _eSwipe.UP;          // 45 ~ 135
                case 2:
                    return _eSwipe.LEFT;        // 135 ~ 225
                case 3:
                    return _eSwipe.DOWN;        // 225 ~ 315
            }

            return _eSwipe.NONE;
        }

        // vtStart와 vtEnd 사이의 각도를 구하기
        static float EvalDragAngle(Vector2 vtStart, Vector2 vtEnd)
        {
            Vector2 dragDirection = vtEnd - vtStart;
            if(dragDirection.magnitude <= 0.2f)
            {
                return -1f;
            }

            //Atan(Arc Tangent) : 탄젠트의 역함수, y와 x 사이의 각도를 구할 때 사용, -90도 ~ 90도 사이의 라디안 값으로만 반환
            //Atan2 함수로 -180도 ~ 180도 사이의 라디안 값으로 반환할 수 있어 각도를 구할 수 있음
            float aimAngle = Mathf.Atan2(dragDirection.y, dragDirection.x);
            if(aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;     // 반환된 라디안 값이 음수인 경우 양수값으로 변환
            }

            return aimAngle * Mathf.Rad2Deg;
        }
    }
}
