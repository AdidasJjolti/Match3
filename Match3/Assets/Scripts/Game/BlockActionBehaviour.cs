using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Scriptable;

namespace Match3.Board
{
    public class BlockActionBehaviour : MonoBehaviour
    {
        [SerializeField] BlockConfig _blockConfig;

        public bool _isMoving
        {
            get;
            set;
        }

        Queue<Vector3> _movementQueue = new Queue<Vector3>();

        public void MoveDrop(Vector2 vtDropDistance)
        {
            _movementQueue.Enqueue(new Vector3(vtDropDistance.x, vtDropDistance.y, 1));

            if(!_isMoving)
            {
                StartCoroutine(DoActionMoveDrop());
            }
        }

        IEnumerator DoActionMoveDrop(float acc = 1.0f)
        {
            _isMoving = true;

            while(_movementQueue.Count > 0)
            {
                Vector2 vtDestination = _movementQueue.Dequeue();

                int dropIndex = Mathf.Min(9, Mathf.Max(1, (int)Mathf.Abs(vtDestination.y)));    // 블럭이 떨어지는 거리를 1과 9사이에서 결정
                float duration = _blockConfig.dropSpeed[dropIndex - 1];                         // 떨어지는 거리에 따라 속도 결정

                yield return CoStartDropSmooth(vtDestination, duration * acc);
            }

            _isMoving = false;
            yield break;
        }

        IEnumerator CoStartDropSmooth(Vector2 vtDropDistance, float duration)
        {
            Vector2 to = new Vector3(transform.position.x + vtDropDistance.x, transform.position.y - vtDropDistance.y);
            yield return Action2D.MoveTo(transform, to, duration);
        }
    }
}
