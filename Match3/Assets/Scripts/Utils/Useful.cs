using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class SortedListMethods
    {
        // 첫번째 노드의 key-value 페어를 구하는 확장 메소드
        public static KeyValuePair<T1, T2> First<T1, T2>(this SortedList<T1, T2> sortedList)
        {
            if(sortedList.Count == 0)
            {
                return new KeyValuePair<T1, T2>();
            }

            return new KeyValuePair<T1, T2>(sortedList.Keys[0], sortedList.Values[0]);
        }
    }

    public class ReverseComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            // Reverse the comparison to sort in descending order
            return Comparer<T>.Default.Compare(y, x);
        }
    }
}
