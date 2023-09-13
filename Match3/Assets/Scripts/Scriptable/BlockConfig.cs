using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Board;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Match3/Block Config", fileName = "BlockConfig.asset")]
    public class BlockConfig : ScriptableObject
    {
        public Sprite[] basicBlockSprites;
        public float[] dropSpeed;           // �������� �Ÿ��� ���� �ٸ� �ӵ� ����
        public GameObject explosion;

        public GameObject GetExplosionObject(_eBlockQuestType questType)
        {
            switch(questType)
            {
                case _eBlockQuestType.CLEAR_SIMPLE:
                    return Instantiate(explosion) as GameObject;
                default:
                    return Instantiate(explosion) as GameObject;
            }
        }
    }
}

