using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Match3.Stage
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] G_TileMap2D _tilemap2D;                     // MapData ������ �������� ���� �����ϱ� ���� Ÿ�ϸ�, Board ��� ���
        [SerializeField] G_CameraController _cameraController;       // ī�޶� �þ� ������ ���� ���� ī�޶�

        bool _Init;     // �ʱ�ȭ ���� üũ
        Stage _stage;   // Stage ���� ����
        int _stageNumber;
        public static MapData _data;

        InputManager _inputManager;
        bool _isTouchDown;          //�Է� ����, ��ȿ�� ���� Ŭ���ϸ� true
        BlockPos _blockDownPos;     //���忡 ����� ���� ��ġ
        Vector3 _clickPos;          //���� ���� ���� ��ǥ�� ǥ���� Down ��ġ

        ActionManager _actionManager;

        void Awake()
        {
            MapDataLoader _mapDataLoader = new MapDataLoader();
            MapData mapData = _mapDataLoader.Load($"stage{_stageNumber+1}");              // stage1 ������ ������ �ҷ�����
            _tilemap2D.GenerateTileMap(mapData);                                          // mapData�� �������� Ÿ�� ����
            _cameraController.SetupCamera();                                              // Ÿ�ϸ� ũ�⿡ �°� ī�޶� �þ� �缳��
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

            OnInputHandler();       // �Է� ������ �޼���
        }

        void InitStage()
        {
            if (_Init)
            {
                return;
            }

            _Init = true;
            _inputManager = new InputManager(_tilemap2D.transform);     // InputManager ��ü ����
            _stageNumber = 0;

            BuildStage();   // �������� ������ ���� ȣ��
            _stage.PrintAll();
        }

        void BuildStage()
        {
            _stage = StageBuilder.BuildStage(nStage : _stageNumber, _tilemap2D);
            _actionManager = new ActionManager(_tilemap2D.transform, _stage);

            if (_stage != null)
            {
                _stage.board.ComposeStage();
            }
        }

        void OnInputHandler()
        {
            // ���콺 ��ư Ŭ�� ������ ��ǥ ���
            if(!_isTouchDown && _inputManager._isTouchDown)
            {
                // ���� ���� ���� ��ǥ ���ϱ�
                // 10x10 ���� ����, x�� y ��ǥ�� -5 ~ 5���̿��� ����
                Vector2 point = _inputManager._touch2BoardPosition;
                Debug.Log("local point: " + point);

                if(!_stage.IsInsideBoard(point))    // ���� ���� Ŭ������ ���� ��� ����
                {
                    Debug.Log("Not Inside the Board");
                    return;
                }

                BlockPos blockPos;

                if(_stage.IsOnValideBlock(point, out blockPos))     // �������� ������ ���� Ŭ���� ��� ó��
                {
                    _isTouchDown = true;                            // Ŭ�� ���� on
                    _blockDownPos = blockPos;                       // Ŭ���� �� ��ġ
                    _clickPos = point;                              // Ŭ���� ���� ��ǥ
                }
            }
            // ���콺 ��ư Ŭ�� �� ���� ���� ������ ��ǥ ���
            else if(_isTouchDown && _inputManager._isTouchUp)
            {
                Vector2 point = _inputManager._touch2BoardPosition;
                _eSwipe swipeDir = _inputManager.EvalSwipeDir(_clickPos, point);
                Debug.Log($"Swipe = {swipeDir}, Block = {_blockDownPos}");

                if(swipeDir != _eSwipe.NONE)
                {
                    _actionManager.DoSwipeAction(_blockDownPos.row, _blockDownPos.col, swipeDir);       // �������� �׼� ��û
                }

                _isTouchDown = false;
            }
        }
    }
}

