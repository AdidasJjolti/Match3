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

            // Self Destroy ����� ��� ���� ������Ʈ ����
            if(selfRemove)
            {
                Object.Destroy(target.gameObject, 0.1f);
            }

            yield break;
        }

        public static IEnumerator Scale(Transform target, float toScale, float speed)   // target : �ִϸ��̼� ��� ������Ʈ, toScale : Ŀ���ų� �پ��� ���� ũ��, speed : ũ�� ���� �ӵ�
        {
            bool isIncrease = target.localScale.x < toScale;        // ���� ũ�⺸�� toScale�� ū�� �Ǵ�, true�� Ȯ���ϰ� false�� ���
            float dir = isIncrease ? 1 : -1;                        // Ȯ���ϸ� 1, ����ϸ� -1 �Ҵ�

            float factor;
            while(true)
            {
                factor = Time.deltaTime * speed * dir;
                target.localScale = new Vector3(target.localScale.x + factor, target.localScale.y + factor, target.localScale.z);

                // Ȯ�� �Ǵ� ����Ͽ� toScale ũ�⿡ �����ϸ� ����
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
