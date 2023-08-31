using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class InputManager
    {
        Transform _container;

        // 전처리기 : 특정 플랫폼에 따라 스크립트를 컴파일할지 말지 결정
#if UNITY_ANDROID && !UNITY_EDITOR
        IInputHandlerBase _inputHandler = new TouchHandler();
#else
        IInputHandlerBase _inputHandler = new MouseHandler();
#endif
        public InputManager(Transform container)
        {
            _container = container;
        }

        public bool _isTouchDown => _inputHandler._isInputDown;
        public bool _isTouchUp => _inputHandler._isInputUp;
        public Vector2 _touchPosition => _inputHandler._inputPosition;
        public Vector2 _touch2BoardPosition => TouchToPosition(_inputHandler._inputPosition);       // Board의 원점을 기준으로 입력 좌표를 월드 좌표에서 로컬 좌표로 변환한 값 저장

        Vector2 TouchToPosition(Vector3 vtInput)        // vtInput : 스크린 좌표,  vtContainerLocal : 로컬 좌표
        {
            Vector3 vtMousePosW = Camera.main.ScreenToWorldPoint(vtInput);
            Vector3 vtContainerLocal = _container.transform.InverseTransformPoint(vtMousePosW);

            return vtContainerLocal;
        }
    }
}
