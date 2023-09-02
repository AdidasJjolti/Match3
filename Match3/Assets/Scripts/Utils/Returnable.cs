using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    // 코루틴 결과를 수신하기 위한 클래스
    public class Returnable<T>
    {
        public T value
        {
            get;
            set;
        }

        public Returnable(T value)
        {
            this.value = value;
        }
    }
}
