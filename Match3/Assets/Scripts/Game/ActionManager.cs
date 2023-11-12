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

            bool isValidSwipe = _stage.IsValideSwipe(nRow, nCol, swipeDir);

            // ��ȿ�� ���������̰ų� Ŭ���� ����� �������� ��� if ���� true �Ǵ�
            if (isValidSwipe || (!isValidSwipe && CheckItemBlock(nRow, nCol)))
            {
                Debug.Log("��ȿ�� ���������̰ų� Ŭ���� ����� �������̾�.");
                StartCoroutine(CoDoSwipeAction(nRow, nCol, swipeDir));
            }
        }

        public bool CheckItemBlock(int x, int y)
        {
            var block = _stage.board.blocks[x, y];
            return block.type == Board._eBlockType.ITEM && (block.breed > Board._eBlockBreed.ITEM && block.breed < Board._eBlockBreed.ITEM_MAX);
        }

        IEnumerator CoDoSwipeAction(int nRow, int nCol, _eSwipe swipeDir)
        {
            if(!_isRunning)
            {
                _isRunning = true;

                SoundManager._instance.PlayOneShot(_eClip.CHOMP);

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

        bool isFirst = true;

        // 3��ġ �� ����, �� �� �ڸ��� �� �� ��� ����
        IEnumerator EvaluateBoard(Returnable<bool> matchResult)
        {
            // ��Ī�� ���� �ִ� ��� �ݺ� ����
            while(true)
            {
                // ��ġ �� ����
                Returnable<bool> blockMatched = new Returnable<bool>(false);
                yield return StartCoroutine(_stage.Evaluate(blockMatched));

                // ��ġ ���� �ִ� ��� �� ��� �� ��ó�� ����
                if (blockMatched.value)
                {
                    matchResult.value = true;

                    SoundManager._instance.PlayOneShot(_eClip.BLOCKCLEAR);

                    // ��ġ �� ���� �� �� �� ��� �� �� �� ����
                    yield return StartCoroutine(_stage.PostprocessAfterEvaluate());
                }
                // ��ġ ���� ���� ��� while�� ����
                else
                {
                    break;
                }
            }

            yield break;
        }
    }
}