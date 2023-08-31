using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class InputManager
    {
        Transform _container;

        // ��ó���� : Ư�� �÷����� ���� ��ũ��Ʈ�� ���������� ���� ����
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
        public Vector2 _touch2BoardPosition => TouchToPosition(_inputHandler._inputPosition);       // Board�� ������ �������� �Է� ��ǥ�� ���� ��ǥ���� ���� ��ǥ�� ��ȯ�� �� ����

        Vector2 TouchToPosition(Vector3 vtInput)        // vtInput : ��ũ�� ��ǥ,  vtContainerLocal : ���� ��ǥ
        {
            Vector3 vtMousePosW = Camera.main.ScreenToWorldPoint(vtInput);
            Vector3 vtContainerLocal = _container.transform.InverseTransformPoint(vtMousePosW);

            return vtContainerLocal;
        }
    }
}
