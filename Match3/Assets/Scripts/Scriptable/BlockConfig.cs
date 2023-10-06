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
        public Sprite[] itemBlockSprites;
        public float[] dropSpeed;           // 떨어지는 거리에 따라 다른 속도 적용
        public GameObject explosion;
        public Color[] blockColors;

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

        public Color GetBlockColor(_eBlockBreed breed)
        {
            return blockColors[(int)breed];
        }
    }
}

