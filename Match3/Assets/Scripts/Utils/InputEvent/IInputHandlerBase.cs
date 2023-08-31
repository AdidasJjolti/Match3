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

        Vector2 _inputPosition      // Down, Up�� �߻��� ��ũ�� ��ǥ
        {
            get;
        }
    }
}
