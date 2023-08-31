using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class MouseHandler : IInputHandlerBase
    {
        bool IInputHandlerBase._isInputDown => Input.GetButtonDown("Fire1");        // 마우스 왼쪽 버튼 클릭하는 순간에만 true 반환
        bool IInputHandlerBase._isInputUp => Input.GetButtonUp("Fire1");            // 마우스 왼쪽 버튼에서 손가락을 뗄 때 true 반환
        Vector2 IInputHandlerBase._inputPosition => Input.mousePosition;            // 마우스 동작이 발생한 위치 반환
    }
}
