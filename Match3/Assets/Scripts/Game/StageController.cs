using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using UnityEngine.SceneManagement;
using Match3.Board;

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

        Match3.Board.Board _board;

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

            if(Input.GetKeyDown(KeyCode.F12))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

            _board = _stage.board;
        }

        void BuildStage()
        {
            _stage = StageBuilder.BuildStage(nStage : _stageNumber, _tilemap2D);
            _actionManager = new ActionManager(_tilemap2D.transform, _stage, this);

            if (_stage != null)
            {
                _stage.board.ComposeStage(_tilemap2D);
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

                    Debug.Log($"Ŭ���� ��ġ�� ��� ��ǥ�� {_blockDownPos.row}, {_blockDownPos.col}");
                }
            }
            // ���콺 ��ư Ŭ�� �� ���� ���� ������ ��ǥ ���
            else if(_isTouchDown && _inputManager._isTouchUp)
            {
                Vector2 point = _inputManager._touch2BoardPosition;
                _eSwipe swipeDir = _inputManager.EvalSwipeDir(_clickPos, point);

                Vector2 pos = new Vector2(point.x + 5, point.y * -1 + 5);
                Debug.Log($"���콺 ��ư�� �� ��ġ�� ��� ��ǥ�� {(int)pos.x}, {(int)pos.y}");

                Debug.Log($"Swipe = {swipeDir}, Block = {_blockDownPos}");

                if(swipeDir != _eSwipe.NONE)
                {
                    // ToDo : ���������� �� ������ ��� ó��
                    _actionManager.DoSwipeAction(_blockDownPos.row, _blockDownPos.col, swipeDir);       // �������� �׼� ��û
                }
                else
                {
                    // �̹� üũ�� ������ ����� ����, ������ ��� ȿ�� ���� ���� ����Ʈ�� �ִ� ������� �켱 üũ�Ͽ� ���� �����÷ο츦 ����
                    List<Block> checkBlocks = new List<Block>();

                    // Ŭ���� ��ǥ�� ��ġ�� ��� ������ origin���� ����
                    var origin = _board.blocks[(int)pos.x, (int)pos.y];

                    if (origin.breed > _eBlockBreed.ITEM && origin.breed < _eBlockBreed.ITEM_MAX)
                    {
                        Debug.Log("������ ��� ����");

                        ChangeAffectedBlocks((int)pos.x, (int)pos.y, origin.breed, checkBlocks);

                        _actionManager.DoSwipeAction((int)pos.x, (int)pos.y, swipeDir);

                        // ����� �����ϱ� ���� ����
                        // 1. ���������� �ƴ� ���� EvaluateBoard�� �����Ͽ� �� ���� ó��
                        // 2. Ŭ���� ����� ������ Ÿ������ �Ǵ��ϰ� �������̸� swipeDir == _eSwipe.NONE�� ���� �������� �׼��� ó���ϵ��� ����
                        // 3. �������� ������ ������ ��� ����� _eBlockStatus�� MATCH�� ����
                        // 4. �������� ������ ������ ��� ����� _breed�� ��� �����ϰ� ����, ��� ����� �������� ����
                    }
                }


                _isTouchDown = false;
            }
        }

        // ������ ȿ�� ���� ���� ���� ��� �߿��� ������ ����� �ִ� ��� �߰� ó�� ����
        public void ChangeAffectedBlocks(int x, int y, _eBlockBreed originBreed, List<Block> checkBlocks)
        {
            checkBlocks.Add(_board.blocks[x, y]);

            switch (_board.blocks[x, y]._breed)
            {
                case _eBlockBreed.VERTICAL:

                    for (int i = 0; i < _board._Row; i++)
                    {
                        if(checkBlocks.Contains(_board.blocks[x, i]))
                        {
                            continue;
                        }

                        if (_board.blocks[x, i].type == _eBlockType.BASIC)
                        {
                            _board.blocks[x, i].breed = originBreed;
                        }
                        else if (_board.blocks[x, i].type == _eBlockType.ITEM)
                        {
                            ChangeAffectedBlocks(x, i, originBreed, checkBlocks);
                        }
                    }
                    break;

                case _eBlockBreed.HORIZONTAL:

                    for (int i = 0; i < _board._Col; i++)
                    {
                        if (checkBlocks.Contains(_board.blocks[i, y]))
                        {
                            continue;
                        }

                        if (_board.blocks[i, y].type == _eBlockType.BASIC)
                        {
                            _board.blocks[i, y].breed = originBreed;
                        }
                        else if (_board.blocks[i, y].type == _eBlockType.ITEM)
                        {
                            ChangeAffectedBlocks(i, y, originBreed, checkBlocks);
                        }
                    }
                    break;

                case _eBlockBreed.BOMB:

                    for (int i = Mathf.Max(0, x - 1); i < Mathf.Min(_board._Row, x + 2); i++)
                    {
                        for (int j = Mathf.Max(0, y - 1); j < Mathf.Min(_board._Col, y + 2); j++)
                        {
                            if (checkBlocks.Contains(_board.blocks[i, j]))
                            {
                                continue;
                            }

                            if (_board.blocks[i, j].type == _eBlockType.BASIC)
                            {
                                _board.blocks[i, j].breed = originBreed;
                            }
                            else if (_board.blocks[i, j].type == _eBlockType.ITEM)
                            {
                                ChangeAffectedBlocks(i, j, originBreed, checkBlocks);
                            }
                        }
                    }
                    break;
            }

            _board.blocks[x, y].breed = originBreed;
        }
    }
}

