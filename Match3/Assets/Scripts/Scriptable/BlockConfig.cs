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
        public float[] dropSpeed;           // �������� �Ÿ��� ���� �ٸ� �ӵ� ����
        public GameObject explosion;
        public Color[] blockColors;

        public GameObject GetExplosionObject(_eBlockQuestType questType)
        {
            //ToDo : explosion ���� ������Ʈ�� ��� �ִ� Ǯ�� �����ؼ� ���� �ִ� explosion�� ��������
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

