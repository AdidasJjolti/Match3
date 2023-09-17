using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;

namespace Match3.Board
{
    public class BlockBehaviour : MonoBehaviour
    {
        [SerializeField] Block _block;
        [SerializeField] SpriteRenderer _renderer;
        [SerializeField] BlockConfig _blockConfig;

        void Start()
        {
            //_block = GetComponent<Block>();
            //_renderer = GetComponent<SpriteRenderer>();
            //UpdateView(false);    // Ÿ�Կ� ���� ������ ��������Ʈ ����ϵ��� ȣ��, �ʱ� ������ �� false�� ���ڷ� ����
        }


        // Block ��ü ������ ����
        //internal void SetBlock(Block block)
        //{
        //    _block = block;
        //}

        // Ÿ���� EMPTY�� ��� ��������Ʈ�� null�� ����
        public void UpdateView(bool valueChange)
        {
            if(_block.type == _eBlockType.EMPTY)
            {
                _renderer.sprite = null;
            }
            else
            {
                _block.breed = (_eBlockBreed)Random.Range(0, (int)_eBlockBreed.MAX);
                _renderer.sprite = _blockConfig.basicBlockSprites[(int)_block.breed];
            }

            Debug.Log("�� ���� ���� �Ϸ�");
        }

        public void DoActionClear()
        {
            StartCoroutine(CoStartSimpleExplosion(true));
        }

        IEnumerator CoStartSimpleExplosion(bool destroy = true)     // ����Ʈ �Ű� ������ true ����, ȣ���� �� �Ķ���͸� ���� �ʴ� ��� true�� ����, ����Ʈ �Ű� ������ �Ű� ���� �� ���ʿ��� ��ġ ����
        {
            yield return Util.Action2D.Scale(transform, Core.Constants.BLOCK_DESTROY_SCALE, 4f);

            GameObject explosionObj = _blockConfig.GetExplosionObject(_eBlockQuestType.CLEAR_SIMPLE);
            ParticleSystem.MainModule newModule = explosionObj.GetComponent<ParticleSystem>().main;
            newModule.startColor = _blockConfig.GetBlockColor(_block._breed);

            explosionObj.SetActive(true);
            explosionObj.transform.position = this.transform.position;
            Debug.Log($"Explosion positions : {explosionObj.transform.position.x}, {explosionObj.transform.position.y}");

            yield return new WaitForSeconds(0.1f);

            if(destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Assert(false, "Unknown Action : GameObject No destroy After Particle");
            }
        }
    }
}

