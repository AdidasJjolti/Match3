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
            //ToDo : explosion 게임 오브젝트를 담고 있는 풀에 접근해서 남아 있는 explosion을 가져오기
            switch(questType)
            {
                case _eBlockQuestType.CLEAR_SIMPLE:
                    //return Instantiate(explosion) as GameObject;
                    return PoolManager.Instance.PoolOut();
                default:
                    //return Instantiate(explosion) as GameObject;
                    return PoolManager.Instance.PoolOut();
            }
        }

        public Color GetBlockColor(_eBlockBreed breed)
        {
            return blockColors[(int)breed];
        }
    }
}

