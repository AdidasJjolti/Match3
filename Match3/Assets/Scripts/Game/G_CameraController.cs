using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_CameraController : MonoBehaviour
{
    [SerializeField] G_TileMap2D _tilemap2D;    // �� ũ�� ������ �ҷ����� ���� Tilemap2D
    Camera _mainCamera;                         // ī�޶� �þ� ������ ���� ���� ī�޶�

    float _wDelta = 0.9f;   // ���� �þ� ������
    float _hDelta = 0.6f;   // ���� �þ� ������
    float _maxViewSize;     // ī�޶� �þ� �ִ� ũ��


    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    public void SetupCamera()
    {
        int width = _tilemap2D.GetWidth();
        int height = _tilemap2D.GetHeight();

        // ī�޶� �þ� ����, ��ü ���� ȭ�鿡 �������� ����, ���� ȭ�鿡 �°� �ڵ� ����
        float size = (width >= height) ? width * _wDelta : height * _hDelta;   // ���ΰ� �� ��� ���� �ʺ� �þ� ������ ����, �ݴ�� ���� �ʺ� �þ� ������ ����

        if (_mainCamera == null)
        {
            _mainCamera = GetComponent<Camera>();
        }
        _mainCamera.orthographicSize = size;
        Debug.Log($"Camera size : {_mainCamera.orthographicSize}");

        // ī�޶� y �� ��ǥ ����
        if (height > width)
        {
            // ���̰� �ʺ񺸴� �� ū ��� ī�޶��� y�� ��ġ�� ����
            Vector3 position = new Vector3(0, 0.05f, -10);
            position.y *= height;
            transform.position = position;
        }

        _maxViewSize = _mainCamera.orthographicSize;
    }
}
