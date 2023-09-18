using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using UnityEngine.SceneManagement;

namespace Match3.Stage
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] G_TileMap2D _tilemap2D;                     // MapData 정보를 바탕으로 맵을 생성하기 위한 타일맵, Board 대신 사용
        [SerializeField] G_CameraController _cameraController;       // 카메라 시야 설정을 위한 메인 카메라

        bool _Init;     // 초기화 상태 체크
        Stage _stage;   // Stage 변수 선언
        int _stageNumber;
        public static MapData _data;

        InputManager _inputManager;
        bool _isTouchDown;          //입력 상태, 유효한 블럭을 클릭하면 true
        BlockPos _blockDownPos;     //보드에 저장된 블럭의 위치
        Vector3 _clickPos;          //보드 기준 로컬 좌표로 표현한 Down 위치

        ActionManager _actionManager;

        void Awake()
        {
            MapDataLoader _mapDataLoader = new MapDataLoader();
            MapData mapData = _mapDataLoader.Load($"stage{_stageNumber+1}");              // stage1 데이터 파일을 불러오기
            _tilemap2D.GenerateTileMap(mapData);                                          // mapData를 바탕으로 타일 생성
            _cameraController.SetupCamera();                                              // 타일맵 크기에 맞게 카메라 시야 재설정
            _data = mapData;
        }

        void Start()
        {
            InitStage();
        }

        void Update()
        {
            if(!_Init)
            {
                return;
            }

            if(Input.GetKeyDown(KeyCode.F12))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            OnInputHandler();       // 입력 감지할 메서드
        }

        void InitStage()
        {
            if (_Init)
            {
                return;
            }

            _Init = true;
            _inputManager = new InputManager(_tilemap2D.transform);     // InputManager 객체 생성
            _stageNumber = 0;

            BuildStage();   // 스테이지 구성을 위해 호출
            _stage.PrintAll();
        }

        void BuildStage()
        {
            _stage = StageBuilder.BuildStage(nStage : _stageNumber, _tilemap2D);
            _actionManager = new ActionManager(_tilemap2D.transform, _stage);

            if (_stage != null)
            {
                _stage.board.ComposeStage(_tilemap2D);
            }
        }

        void OnInputHandler()
        {
            // 마우스 버튼 클릭 시점의 좌표 출력
            if(!_isTouchDown && _inputManager._isTouchDown)
            {
                // 보드 기준 로컬 좌표 구하기
                // 10x10 보드 기준, x와 y 좌표는 -5 ~ 5사이에서 결정
                Vector2 point = _inputManager._touch2BoardPosition;
                Debug.Log("local point: " + point);

                if(!_stage.IsInsideBoard(point))    // 보드 안을 클릭하지 않은 경우 무시
                {
                    Debug.Log("Not Inside the Board");
                    return;
                }

                BlockPos blockPos;

                if(_stage.IsOnValideBlock(point, out blockPos))     // 스와이프 가능한 블럭을 클릭한 경우 처리
                {
                    _isTouchDown = true;                            // 클릭 상태 on
                    _blockDownPos = blockPos;                       // 클릭한 블럭 위치
                    _clickPos = point;                              // 클릭한 로컬 좌표
                }
            }
            // 마우스 버튼 클릭 후 손을 떼는 시점의 좌표 출력
            else if(_isTouchDown && _inputManager._isTouchUp)
            {
                Vector2 point = _inputManager._touch2BoardPosition;
                _eSwipe swipeDir = _inputManager.EvalSwipeDir(_clickPos, point);
                Debug.Log($"Swipe = {swipeDir}, Block = {_blockDownPos}");

                if(swipeDir != _eSwipe.NONE)
                {
                    _actionManager.DoSwipeAction(_blockDownPos.row, _blockDownPos.col, swipeDir);       // 스와이프 액션 요청
                }

                _isTouchDown = false;
            }
        }
    }
}

