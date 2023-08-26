using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Board;

public class InputController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    _eTileType _currentType = _eTileType.EMPTY;    // ����Ʈ �������� ���� Ÿ�� Ÿ������ EMPTY(0��) Ÿ�� ����

    [SerializeField] CameraController _cameraController;
    Vector2 _prevMousePos;
    Vector2 _curMousePos;

    void Update()
    {
        // ���콺�� UI�� Ŭ���ϸ� �Ʒ� �ڵ� �������� ����
        if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UpdateCamera();

        RaycastHit2D hit;

        if(Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -1 * _mainCamera.transform.position.z;   // ī�޶�� ��ũ�� ��ǥ ������ �Ÿ� ����, ī�޶��� ��ġ�� ������ ��ȯ�Ͽ� ���콺 ��ũ�� ��ǥ z���� ī�޶�� ��ġȭ

            Vector2 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);   // 2D���� ScreenPointToRay ��ſ� ScreenToWorldPoint�� ���
            hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if(hit.collider != null)
            {
                Tile tile = hit.transform.GetComponent<Tile>();

                if(tile != null)
                {
                    tile.TileType = _currentType;
                }
            }

            //Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);   // ī�޷κ��� ȭ���� ���콺 ��ġ�� �����ϴ� ray ����
            
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            //{
            //    Tile tile = hit.transform.GetComponent<Tile>();
            //    // raycast�� �ε��� ������Ʈ(Ÿ�� ����� ��ư)�� �ִ� ��� Ÿ�� Ÿ�� �Ӽ����� ����
            //    if(tile != null)
            //    {
            //        tile.TileType = _currentType;
            //    }
            //}
        }
    }

    public void SetTileType(int type)
    {
        _currentType = (_eTileType)type;   // ���콺 Ŭ������ ���� Ÿ�� Ÿ���� �����ϴ� �ڵ�
    }

    public void UpdateCamera()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        _cameraController.SetPosition(x, y);     // Ű���� �Է��� �޾Ƽ� ī�޶� ���� ��ġ���� x �Ǵ� y��ŭ �̵�

        if(Input.GetMouseButtonDown(2))
        {
            _curMousePos = _prevMousePos = Input.mousePosition;   // ���콺 ���� ������ �� ���� ���콺 ��ġ ����
        }
        else if(Input.GetMouseButton(2))
        {
            _curMousePos = Input.mousePosition;    // ���콺 ���� ���� �̵��� ��� ���� ��ġ ����
            if(_prevMousePos != _curMousePos)
            {
                // ������ �Ÿ��� 0.5�踸ŭ ī�޶� ��ġ �̵�
                Vector2 move = (_prevMousePos - _curMousePos) * 0.5f;
                _cameraController.SetPosition(move.x, move.y);
            }
        }
        _prevMousePos = _curMousePos;  // �̵��� ������ ���� ���콺 ��ġ�� ���� ���콺 ��ġ�� ��ġȭ

        float distance = Input.GetAxisRaw("Mouse ScrollWheel");  // ���� ���� �����̸� ����, �Ʒ��� �����̸� �ܾƿ�
        _cameraController.SetOrthographicSize(-distance);
    }
}
