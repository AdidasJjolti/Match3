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
        bool _isRunning;                // �������� �׼� ���� ����

        public ActionManager(Transform container, Stage stage)
        {
            _container = container;
            _stage = stage;
            _monoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
        }

        // �ڷ�ƾ Wrapper �޼ҵ�
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _monoBehaviour.StartCoroutine(routine);
        }

        public void DoSwipeAction(int nRow, int nCol, _eSwipe swipeDir)
        {
            Debug.Assert(nRow >= 0 && nRow < _stage._Row && nCol >= 0 && nCol < _stage._Col);       // Ŭ���� �� ��ġ�� �������� ���� ���̸� ���� �α� ���

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

                Returnable<bool> swipedBlock = new Returnable<bool>(false);             // �ڷ�ƾ ���� ����� ���޹��� Returnable ��ü ����
                yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);

                if(swipedBlock.value)
                {
                    Returnable<bool> matchBlock = new Returnable<bool>(false);
                    yield return EvaluateBoard(matchBlock);

                    // ���������� ���� 3��ġ �������� �ʴ� ��� �����·� ����
                    if(!matchBlock.value)
                    {
                        yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);
                    }
                }

                _isRunning = false;
            }

            yield break;
        }

        // 3��ġ �� ����, �� �� �ڸ��� �� �� ��� ����
        IEnumerator EvaluateBoard(Returnable<bool> matchResult)
        {
            yield return _stage.Evaluate(matchResult);      // 3��ġ �� ����

            //�� ���� �� �� �� ��� �� �� �� ����
            if(matchResult.value)
            {
                yield return _stage.PostprocessAfterEvaluate();
            }
        }
    }
}