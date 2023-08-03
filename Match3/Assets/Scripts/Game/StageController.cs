using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Stage
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] G_TileMap2D _tilemap2D;                     // MapData 정보를 바탕으로 맵을 생성하기 위한 타일맵
        [SerializeField] G_CameraController _cameraController;       // 카메라 시야 설정을 위한 메인 카메라

        bool _Init;     // 초기화 상태 체크
                        //Stage _stage;   // Stage 변수 선언

        void Awake()
        {
            MapDataLoader _mapDataLoader = new MapDataLoader();
            MapData mapData = _mapDataLoader.Load("stage1");              // stage1 데이터 파일을 불러오기
            _tilemap2D.GenerateTileMap(mapData);                          // mapData를 바탕으로 타일 생성
            _cameraController.SetupCamera();                              // 타일맵 크기에 맞게 카메라 시야 재설정
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
        }
    }
}

