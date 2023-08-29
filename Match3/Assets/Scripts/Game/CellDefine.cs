using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public enum _eTileType
    {
        EMPTY = 0,
        GRASS,      // ����� ���� ��ġ���� ����, ��ǻ� EMPTY
        NORMAL,     // ���� �ִ� ���� �� ������ �� �� ����
        HARD,       // ���� �ִ� ���� �� �� ����

        MAX
    }

    static class CellTypeMethod
    {
        // ���� ��ġ�� �� �ִ� Ÿ������ üũ
        public static bool IsBlockAllocatableType(this _eTileType type)
        {
            return (type == _eTileType.NORMAL);
        }

        // ���� �̵� ������ Ÿ������ üũ
        public static bool IsBlockMovableType(this _eTileType type)
        {
            return (type == _eTileType.NORMAL);
        }
    }

}
