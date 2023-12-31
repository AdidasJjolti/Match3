using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_CameraController : MonoBehaviour
{
    [SerializeField] G_TileMap2D _tilemap2D;    // 맵 크기 정보를 불러오기 위한 Tilemap2D
    Camera _mainCamera;                         // 카메라 시야 설정을 위한 메인 카메라

    float _wDelta = 0.9f;   // 가로 시야 보정값
    float _hDelta = 0.6f;   // 세로 시야 보정값
    float _maxViewSize;     // 카메라 시야 최대 크기


    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    public void SetupCamera()
    {
        int width = _tilemap2D.GetWidth();
        int height = _tilemap2D.GetHeight();

        // 카메라 시야 설정, 전체 맵이 화면에 들어오도록 수정, 세로 화면에 맞게 코드 수정
        float size = (width >= height) ? width * _wDelta : height * _hDelta;   // 가로가 더 길면 가로 너비에 시야 보정값 적용, 반대면 세로 너비에 시야 보정값 적용

        if (_mainCamera == null)
        {
            _mainCamera = GetComponent<Camera>();
        }
        _mainCamera.orthographicSize = size;
        Debug.Log($"Camera size : {_mainCamera.orthographicSize}");

        // 카메라 y 축 좌표 설정
        if (height > width)
        {
            // 높이가 너비보다 더 큰 경우 카메라의 y축 위치를 수정
            Vector3 position = new Vector3(0, 0.05f, -10);
            position.y *= height;
            transform.position = position;
        }

        _maxViewSize = _mainCamera.orthographicSize;
    }
}
