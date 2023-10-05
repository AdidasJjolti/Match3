using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public enum _eBlockType
    {
        EMPTY = 0,
        BASIC,
        MAX,

        ITEM = 10,
        HORIZONTAL,
        VERTICAL,
        BOMB,
        ITEM_MAX
    }

    public enum _eBlockBreed
    {
        NONE = -1,
        BREED_0,
        BREED_1,
        BREED_2,
        BREED_3,
        BREED_4,
        BREED_5,

        MAX
    }

    public enum _eBlockStatus
    {
        NONE = -1,
        NORMAL,     // �⺻ ����
        MATCH,      // ��Ī ���� �ִ� ����
        CLEAR,      // Ŭ���� ���� ����

        MAX
    }

    public enum _eBlockQuestType
    {
        NONE = -1,
        CLEAR_SIMPLE,           // ���� �� ����
        CLEAR_HORZ,             // ������ �� ����
        CLEAR_VERT,             // ������ �� ����
        CLEAR_CIRCLE,           // �ֺ� �� ����
        CLEAR_LAZER,            // ������ ���� �� ��� ����
        CLEAR_HORZ_BUFF,        // HORZ, CIRCLE ����
        CLEAR_VERT_BUFF,        // VERT, CIRCLE ����
        CLEAR_CIRCLE_BUFF,      // CIRCLE, CIRCLE ����
        CLEAR_LAZER_BUFF,       // LAZER, LAZER ����

        MAX
    }

    static class BlockMethod
    {
        // block�� null�� �ƴϰ� Ÿ�� ���� ���� breed�� ��� true ����
        public static bool IsSafeEqual(this Block block, Block targetBlock)
        {
            if (block == null)
            {
                return false;
            }

            return block.IsEqual(targetBlock);
        }
    }
}
