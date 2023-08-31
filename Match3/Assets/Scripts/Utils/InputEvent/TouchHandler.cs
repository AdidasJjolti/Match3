using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class TouchHandler : IInputHandlerBase
    {
        bool IInputHandlerBase._isInputDown => Input.GetTouch(0).phase == TouchPhase.Began;        // ��ġ ����Ʈ�� ������ �������� true ��ȯ
        bool IInputHandlerBase._isInputUp => Input.GetTouch(0).phase == TouchPhase.Ended;          // ��ġ ����Ʈ�� ������� �� true ��ȯ
        Vector2 IInputHandlerBase._inputPosition => Input.GetTouch(0).position;                    // ��ġ ������ �߻��� ��ġ ��ȯ
    }
}
