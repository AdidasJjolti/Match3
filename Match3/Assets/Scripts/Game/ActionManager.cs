using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Match3.Stage
{
    public class ActionManager
    {
        Transform _container;
        Stage _stage;
        MonoBehaviour _monoBehaviour;
        bool _isRunning;                // 스와이프 액션 실행 상태

        public ActionManager(Transform container, Stage stage)
        {
            _container = container;
            _stage = stage;
            _monoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
        }

        // 코루틴 Wrapper 메소드
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _monoBehaviour.StartCoroutine(routine);
        }

        public void DoSwipeAction(int nRow, int nCol, _eSwipe swipeDir)
        {
            Debug.Assert(nRow >= 0 && nRow < _stage._Row && nCol >= 0 && nCol < _stage._Col);       // 클릭한 블럭 위치가 스테이지 범위 밖이면 에러 로그 출력

            if(_stage.IsValideSwipe(nRow, nCol, swipeDir))
            {
                StartCoroutine(CoDoSwipeAction(nRow, nCol, swipeDir));
            }
        }

        IEnumerator CoDoSwipeAction(int nRow, int nCol, _eSwipe swipeDir)
        {
            if(!_isRunning)
            {
                _isRunning = true;

                Returnable<bool> swipedBlock = new Returnable<bool>(false);             // 코루틴 실행 결과를 전달받을 Returnable 객체 생성
                yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);

                if(swipedBlock.value)
                {
                    Returnable<bool> matchBlock = new Returnable<bool>(false);
                    yield return EvaluateBoard(matchBlock);

                    // 스와이프한 블럭이 매치되지 않는 경우 원상태로 복귀
                    if(matchBlock.value)
                    {
                        yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);
                    }
                }

                _isRunning = false;
            }

            yield break;
        }

        // 3매치 블럭 삭제, 빈블럭 자리에 새 블럭 드랍 실행
        IEnumerator EvaluateBoard(Returnable<bool> matchResult)
        {
            yield return _stage.Evaluate(matchResult);
        }
    }
}