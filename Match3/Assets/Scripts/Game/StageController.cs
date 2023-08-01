using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] G_TileMap2D _tilemap2D;                     // MapData ������ �������� ���� �����ϱ� ���� Ÿ�ϸ�
    [SerializeField] G_CameraController _cameraController;       // ī�޶� �þ� ������ ���� ���� ī�޶�

    void Awake()
    {
        MapDataLoader _mapDataLoader = new MapDataLoader();
        MapData mapData = _mapDataLoader.Load("stage1");              // stage1 ������ ������ �ҷ�����
        _tilemap2D.GenerateTileMap(mapData);                          // mapData�� �������� Ÿ�� ����
        _cameraController.SetupCamera();                              // Ÿ�ϸ� ũ�⿡ �°� ī�޶� �þ� �缳��
    }
}
