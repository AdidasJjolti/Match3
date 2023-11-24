using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Match3.Board;

namespace Match3.Stage
{
    public class ActionManager
    {
        Transform _container;
        Stage _stage;
        MonoBehaviour _monoBehaviour;
        StageController _stageController;
        bool _isRunning;                // �������� �׼� ���� ����

        public ActionManager(Transform container, Stage stage, StageController stageController)
        {
            _container = container;
            _stage = stage;
            _monoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
            _stageController = stageController;
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
                Block baseBlock = _stage.board.blocks[nRow, nCol];                      // �������� ���� ����� ������ ����
                Block targetBlock = GetTargetBlock(nRow, nCol, swipeDir);               // �������� Ÿ�� ����� ������ ����

                yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);

                if(swipedBlock.value)
                {
                    Returnable<bool> matchBlock = new Returnable<bool>(false);

                    yield return EvaluateBoard(matchBlock, baseBlock, targetBlock);

                    // ���������� ���� 3��ġ �������� �ʴ� ��� �����·� ����
                    // ��, ���������� ��� �� �ϳ��� ������ ����� ��� �������� ����

                    if(matchBlock.value)
                    {
                        GameManager.Instance.ReduceRemainingMoves();        // ���� ������ Ƚ�� ����
                    }

                    if(!matchBlock.value)
                    {
                        yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);
                    }
                }

                _isRunning = false;
            }

            yield break;
        }

        Block GetTargetBlock(int x, int y, _eSwipe swipeDir)
        {
            // ���������Ǵ� ���� ��ġ ���ϱ�
            int nSwipeRow = x, nSwipeCol = y;
            nSwipeRow += swipeDir.GetTargetCol();
            nSwipeCol += swipeDir.GetTargetRow();

            return _stage.board.blocks[nSwipeRow, nSwipeCol];
        }

        bool isFirst = true;

        // 3��ġ �� ����, �� �� �ڸ��� �� �� ��� ����
        IEnumerator EvaluateBoard(Returnable<bool> matchResult, Block baseBlock, Block targetBlock)
        {
            // ���������� ��� �� �ϳ��� ������ ����� ��� ������ ��� ȿ�� ���� �켱 ����
            if (baseBlock.type == _eBlockType.ITEM || targetBlock.type == _eBlockType.ITEM)
            {
                Debug.Log("���������� ��� �߿� �������� �־�");

                // �̹� üũ�� ������ ����� ����, ������ ��� ȿ�� ���� ���� ����Ʈ�� �ִ� ������� �켱 üũ�Ͽ� ���� �����÷ο츦 ����
                List<Block> checkBlocks = new List<Block>();

                // ���� ��ǥ�� ������ �� �ε����� ��ȯ
                Vector2 baseBlockPos = new Vector2(baseBlock.blockObj.position.x + (_stage._Col / 2.0f), baseBlock.blockObj.position.y * -1 + (_stage._Row / 2.0f));
                Vector2 targetBlockPos = new Vector2(targetBlock.blockObj.position.x + (_stage._Col / 2.0f), targetBlock.blockObj.position.y * -1 + (_stage._Row / 2.0f));

                if (baseBlock.type == _eBlockType.ITEM)
                {
                    _stageController.ChangeAffectedBlocks((int)baseBlockPos.x, (int)baseBlockPos.y, baseBlock.breed, checkBlocks);
                }

                if (targetBlock.type == _eBlockType.ITEM)
                {
                    _stageController.ChangeAffectedBlocks((int)targetBlockPos.x, (int)targetBlockPos.y, targetBlock.breed, checkBlocks);
                }
            }

            // ��Ī�� ���� �ִ� ��� �ݺ� ����
            while (true)
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