using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Tilemap2D _tilemap2D;    // �� ũ�� ������ �ҷ����� ���� Tilemap2D
    Camera _mainCamera;                       // ī�޶� �þ� ������ ���� ���� ī�޶�

    [SerializeField] float _moveSpeed;       // ī�޶� �̵� �ӵ�
    [SerializeField] float _zoomSpeed;       // ī�޶� �� �ӵ�
    [SerializeField] float _minViewSize = 2f; // ī�޶� �þ� �ּ� ũ��
    float _maxViewSize;                       // ī�޶� �þ� �ִ� ũ��

    float _wDelta = 0.9f;   // ���� �þ� ������
    float _hDelta = 0.6f;   // ���� �þ� ������


    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    public void SetupCamera()
    {
        int width = _tilemap2D._width;
        int height = _tilemap2D._height;

        // ī�޶� �þ� ����, ��ü ���� ȭ�鿡 �������� ����, ���� ȭ�鿡 �°� �ڵ� ����
        float size = (width >= height) ? width * _wDelta : height * _hDelta;   // ���ΰ� �� ��� ���� �ʺ� �þ� ������ ����, �ݴ�� ���� �ʺ� �þ� ������ ����
        _mainCamera.orthographicSize = size;
        Debug.Log($"Camera size : {_mainCamera.orthographicSize}");

        // ī�޶� y �� ��ǥ ����
        if(height > width)
        {
            // ���̰� �ʺ񺸴� �� ū ��� ī�޶��� y�� ��ġ�� ����
            Vector3 position = new Vector3(0, 0.05f, -10);
            position.y *= height;
            transform.position = position;
        }

        _maxViewSize = _mainCamera.orthographicSize;
    }

    // Ű���� �Է����� �޾ƿ� ��ǥ��ŭ ī�޶� �̵�
    public void SetPosition (float x, float y)
    {
        transform.position += new Vector3(x, y, 0) * _moveSpeed * Time.deltaTime;
    }

    // ���콺 �ٷ� ī�޶� �þ� ���� ����
    public void SetOrthographicSize (float size)
    {
        if(size == 0)
        {
            return;
        }

        _mainCamera.orthographicSize += size * _zoomSpeed * Time.deltaTime;
        _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize, _minViewSize, _maxViewSize);
    }
}
