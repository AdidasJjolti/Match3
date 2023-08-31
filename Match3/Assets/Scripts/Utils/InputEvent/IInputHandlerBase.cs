using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public interface IInputHandlerBase
    {
        bool _isInputDown
        {
            get;
        }

        bool _isInputUp
        {
            get;
        }

        Vector2 _inputPosition      // Down, Up이 발생한 스크린 좌표
        {
            get;
        }
    }
}
