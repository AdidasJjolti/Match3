using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public enum _eSwipe     //  �������� ���� enum
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
        // �������� ���� ���ϱ�
        public static _eSwipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
        {
            float angle = EvalDragAngle(vtStart, vtEnd);        // �������� ������ ���� �Ǵ�
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

        // vtStart�� vtEnd ������ ������ ���ϱ�
        static float EvalDragAngle(Vector2 vtStart, Vector2 vtEnd)
        {
            Vector2 dragDirection = vtEnd - vtStart;
            if(dragDirection.magnitude <= 0.2f)
            {
                return -1f;
            }

            //Atan(Arc Tangent) : ź��Ʈ�� ���Լ�, y�� x ������ ������ ���� �� ���, -90�� ~ 90�� ������ ���� �����θ� ��ȯ
            //Atan2 �Լ��� -180�� ~ 180�� ������ ���� ������ ��ȯ�� �� �־� ������ ���� �� ����
            float aimAngle = Mathf.Atan2(dragDirection.y, dragDirection.x);
            if(aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;     // ��ȯ�� ���� ���� ������ ��� ��������� ��ȯ
            }

            return aimAngle * Mathf.Rad2Deg;
        }
    }

    public static class SwipeDirMethod
    {
        public static int GetTargetRow(this _eSwipe swipeDir)
        {
            switch (swipeDir)
            {
                case _eSwipe.DOWN:
                    return 1;
                case _eSwipe.UP:
                    return -1;
                default:
                    return 0;
            }
        }

        public static int GetTargetCol(this _eSwipe swipeDir)
        {
            switch (swipeDir)
            {
                case _eSwipe.LEFT:
                    return -1;
                case _eSwipe.RIGHT:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
