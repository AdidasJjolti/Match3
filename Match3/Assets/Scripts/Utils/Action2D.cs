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
    }
}
