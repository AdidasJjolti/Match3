using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class MouseHandler : IInputHandlerBase
    {
        bool IInputHandlerBase._isInputDown => Input.GetButtonDown("Fire1");        // ���콺 ���� ��ư Ŭ���ϴ� �������� true ��ȯ
        bool IInputHandlerBase._isInputUp => Input.GetButtonUp("Fire1");            // ���콺 ���� ��ư���� �հ����� �� �� true ��ȯ
        Vector2 IInputHandlerBase._inputPosition => Input.mousePosition;            // ���콺 ������ �߻��� ��ġ ��ȯ
    }
}
