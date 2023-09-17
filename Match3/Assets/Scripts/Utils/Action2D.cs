using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class Action2D
    {
        public static IEnumerator MoveTo(Transform target, Vector3 to, float duration, bool selfRemove = false)
        {
            Vector2 startPos = target.transform.position;

            float elapsed = 0f;
            while(elapsed < duration)
            {
                elapsed += Time.smoothDeltaTime;
                target.transform.position = Vector2.Lerp(startPos, to, elapsed / duration);

                yield return null;
            }

            target.transform.position = to;

            // Self Destroy 모드인 경우 게임 오브젝트 제거
            if(selfRemove)
            {
                Object.Destroy(target.gameObject, 0.1f);
            }

            yield break;
        }

        public static IEnumerator Scale(Transform target, float toScale, float speed)   // target : 애니메이션 대상 오브젝트, toScale : 커지거나 줄어드는 최종 크기, speed : 크기 변경 속도
        {
            bool isIncrease = target.localScale.x < toScale;        // 현재 크기보다 toScale이 큰지 판단, true면 확장하고 false면 축소
            float dir = isIncrease ? 1 : -1;                        // 확장하면 1, 축소하면 -1 할당

            float factor;
            while(true)
            {
                factor = Time.deltaTime * speed * dir;
                target.localScale = new Vector3(target.localScale.x + factor, target.localScale.y + factor, target.localScale.z);

                // 확장 또는 축소하여 toScale 크기에 도달하면 종료
                if((!isIncrease && target.localScale.x <= toScale) || (isIncrease && target.localScale.x >= toScale))
                {
                    break;
                }

                yield return null;
            }

            yield break;
        }
    }
}
