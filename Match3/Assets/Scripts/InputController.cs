using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    _eTileType _currentType = _eTileType.EMPTY;    // 디폴트 세팅으로 현재 타일 타입으로 EMPTY(0번) 타일 선택

    [SerializeField] CameraController _cameraController;
    Vector2 _prevMousePos;
    Vector2 _curMousePos;

    void Update()
    {
        // 마우스가 UI를 클릭하면 아래 코드 실행하지 않음
        if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UpdateCamera();

        RaycastHit2D hit;

        if(Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -1 * _mainCamera.transform.position.z;   // 카메라와 스크린 좌표 사이의 거리 설정, 카메라의 위치를 음수로 변환하여 마우스 스크린 좌표 z값을 카메라와 일치화

            Vector2 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);   // 2D에선 ScreenPointToRay 대신에 ScreenToWorldPoint를 사용
            hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if(hit.collider != null)
            {
                Tile tile = hit.transform.GetComponent<Tile>();

                if(tile != null)
                {
                    tile.TileType = _currentType;
                }
            }

            //Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);   // 카메로부터 화면의 마우스 위치를 관통하는 ray 생성
            
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            //{
            //    Tile tile = hit.transform.GetComponent<Tile>();
            //    // raycast로 부딪힌 오브젝트(타일 만들기 버튼)가 있는 경우 타일 타입 속성으로 변경
            //    if(tile != null)
            //    {
            //        tile.TileType = _currentType;
            //    }
            //}
        }
    }

    public void SetTileType(int type)
    {
        _currentType = (_eTileType)type;   // 마우스 클릭으로 현재 타일 타입을 변경하는 코드
    }

    public void UpdateCamera()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        _cameraController.SetPosition(x, y);     // 키보드 입력을 받아서 카메라를 현재 위치에서 x 또는 y만큼 이동

        if(Input.GetMouseButtonDown(2))
        {
            _curMousePos = _prevMousePos = Input.mousePosition;   // 마우스 휠을 눌렀을 때 현재 마우스 위치 저장
        }
        else if(Input.GetMouseButton(2))
        {
            _curMousePos = Input.mousePosition;    // 마우스 휠을 눌러 이동한 경우 현재 위치 변경
            if(_prevMousePos != _curMousePos)
            {
                // 움직인 거리의 0.5배만큼 카메라 위치 이동
                Vector2 move = (_prevMousePos - _curMousePos) * 0.5f;
                _cameraController.SetPosition(move.x, move.y);
            }
        }
        _prevMousePos = _curMousePos;  // 이동이 끝나면 이전 마우스 위치를 현재 마우스 위치로 일치화

        float distance = Input.GetAxisRaw("Mouse ScrollWheel");  // 휠을 위로 움직이면 줌인, 아래로 움직이면 줌아웃
        _cameraController.SetOrthographicSize(-distance);
    }
}
