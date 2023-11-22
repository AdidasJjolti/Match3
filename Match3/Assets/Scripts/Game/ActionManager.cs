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
        bool _isRunning;                // 스와이프 액션 실행 상태

        public ActionManager(Transform container, Stage stage, StageController stageController)
        {
            _container = container;
            _stage = stage;
            _monoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
            _stageController = stageController;
        }

        // 코루틴 Wrapper 메소드
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _monoBehaviour.StartCoroutine(routine);
        }

        public void DoSwipeAction(int nRow, int nCol, _eSwipe swipeDir)
        {
            Debug.Assert(nRow >= 0 && nRow < _stage._Row && nCol >= 0 && nCol < _stage._Col);       // 클릭한 블럭 위치가 스테이지 범위 밖이면 에러 로그 출력

            bool isValidSwipe = _stage.IsValideSwipe(nRow, nCol, swipeDir);

            // 유효한 스와이프이거나 클릭한 블록이 아이템인 경우 if 조건 true 판단
            if (isValidSwipe || (!isValidSwipe && CheckItemBlock(nRow, nCol)))
            {
                Debug.Log("유효한 스와이프이거나 클릭한 블록이 아이템이야.");
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

                Returnable<bool> swipedBlock = new Returnable<bool>(false);             // 코루틴 실행 결과를 전달받을 Returnable 객체 생성
                Block baseBlock = _stage.board.blocks[nRow, nCol];                      // 스와이프 시작 블록을 저장할 변수
                Block targetBlock = GetTargetBlock(nRow, nCol, swipeDir);               // 스와이프 타겟 블록을 저장할 변수

                yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, swipedBlock);

                if(swipedBlock.value)
                {
                    Returnable<bool> matchBlock = new Returnable<bool>(false);

                    yield return EvaluateBoard(matchBlock, baseBlock, targetBlock);

                    // 스와이프한 블럭이 3매치 형성되지 않는 경우 원상태로 복귀
                    // 단, 스와이프한 블록 중 하나가 아이템 블록인 경우 실행하지 않음

                    if(matchBlock.value)
                    {
                        GameManager.Instance.ReduceRemainingMoves();        // 남은 움직임 횟수 감소
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
            // 스와이프되는 블럭의 위치 구하기
            int nSwipeRow = x, nSwipeCol = y;
            nSwipeRow += swipeDir.GetTargetCol();
            nSwipeCol += swipeDir.GetTargetRow();

            return _stage.board.blocks[nSwipeRow, nSwipeCol];
        }

        bool isFirst = true;

        // 3매치 블럭 삭제, 빈 블럭 자리에 새 블럭 드랍 실행
        IEnumerator EvaluateBoard(Returnable<bool> matchResult, Block baseBlock, Block targetBlock)
        {
            // ToDo : 스와이프한 블록 중 하나가 아이템 블록인 경우 아이템 블록 효과 로직 우선 실행
            if (baseBlock.type == _eBlockType.ITEM || targetBlock.type == _eBlockType.ITEM)
            {
                Debug.Log("스와이프한 블록 중에 아이템이 있어");

                // 이미 체크한 아이템 블록을 저장, 아이템 블록 효과 실행 전에 리스트에 있는 블록인지 우선 체크하여 스택 오버플로우를 방지
                List<Block> checkBlocks = new List<Block>();

                // ToDo : baseBlock과 targetBlock의 x, y 좌표값을 구하는 메서드 찾기

                // 로컬 좌표를 보드의 블럭 인덱스로 변환
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

            // 매칭된 블럭이 있는 경우 반복 수행
            while (true)
            {
                // 매치 블럭 제거
                Returnable<bool> blockMatched = new Returnable<bool>(false);
                yield return StartCoroutine(_stage.Evaluate(blockMatched));

                // 매치 블럭이 있는 경우 블럭 드롭 등 후처리 진행
                if (blockMatched.value)
                {
                    matchResult.value = true;

                    SoundManager._instance.PlayOneShot(_eClip.BLOCKCLEAR);

                    // 매치 블럭 제거 후 빈 블럭 드롭 후 새 블럭 생성
                    yield return StartCoroutine(_stage.PostprocessAfterEvaluate());
                }
                // 매치 블럭이 없는 경우 while문 종료
                else
                {
                    break;
                }
            }

            yield break;
        }
    }
}