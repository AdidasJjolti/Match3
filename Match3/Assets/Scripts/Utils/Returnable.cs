using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    // �ڷ�ƾ ����� �����ϱ� ���� Ŭ����
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
