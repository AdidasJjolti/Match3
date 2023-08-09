using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Stage
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] G_TileMap2D _tilemap2D;                     // MapData ������ �������� ���� �����ϱ� ���� Ÿ�ϸ�
        [SerializeField] G_CameraController _cameraController;       // ī�޶� �þ� ������ ���� ���� ī�޶�

        bool _Init;     // �ʱ�ȭ ���� üũ
        Stage _stage;   // Stage ���� ����
        int _stageNumber;
        public static MapData _data;

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

        void InitStage()
        {
            if (_Init)
            {
                return;
            }

            _Init = true;
            _stageNumber = 0;

            BuildStage();   // �������� ������ ���� ȣ��
            _stage.PrintAll();
        }

        void BuildStage()
        {
            _stage = StageBuilder.BuildStage(nStage : _stageNumber, row : _tilemap2D.GetWidth(), col : _tilemap2D.GetHeight(), _tilemap2D);
        }
    }
}

