using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class TouchHandler : IInputHandlerBase
    {
        bool IInputHandlerBase._isInputDown => Input.GetTouch(0).phase == TouchPhase.Began;        // 터치 포인트가 눌리는 순간에만 true 반환
        bool IInputHandlerBase._isInputUp => Input.GetTouch(0).phase == TouchPhase.Ended;          // 터치 포인트가 릴리즈될 때 true 반환
        Vector2 IInputHandlerBase._inputPosition => Input.GetTouch(0).position;                    // 터치 동작이 발생한 위치 반환
    }
}
