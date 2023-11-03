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
        // Ÿ���� ITEM�� ��� listSize�� 4�̸� ���� ��Ī�� ��� ���� ������, ���� ��Ī�� ��� ���� �������� ����, listSize�� 5 �̻��̸� ��ź���� ����
        public void UpdateView(List<Block> blockList = null)
        {
            if(_block.type == _eBlockType.EMPTY)
            {
                _renderer.sprite = null;
            }
            else if(_block.type == _eBlockType.BASIC)
            {
                _block.breed = (_eBlockBreed)Random.Range(0, (int)_eBlockBreed.MAX);
                _renderer.sprite = _blockConfig.basicBlockSprites[(int)_block.breed];
            }
            else if(_block.type == _eBlockType.ITEM)
            {
                if(blockList.Count < 5)
                {
                    if(blockList[0].blockObj.position.x == blockList[1].blockObj.position.x)        // ���� ��Ī
                    {
                        _block.breed = _eBlockBreed.HORIZONTAL;
                        _renderer.sprite = _blockConfig.itemBlockSprites[(int)_block.breed - 11];
                    }
                    else
                    {
                        _block.breed = _eBlockBreed.VERTICAL;
                        _renderer.sprite = _blockConfig.itemBlockSprites[(int)_block.breed - 11];
                    }
                }
                else
                {
                    _block.breed = _eBlockBreed.BOMB;
                    _renderer.sprite = _blockConfig.itemBlockSprites[(int)_block.breed - 11];
                }
            }

            //Debug.Log("�� ���� ���� �Ϸ�");
        }

        // Ÿ���� EMPTY�� ��� ��������Ʈ�� null�� ����
        public void ChangeView(_eBlockBreed changeBreed)
        {
            if (_block.type == _eBlockType.EMPTY)
            {
                _renderer.sprite = null;
            }
            else
            {
                _block.breed = changeBreed;
                _renderer.sprite = _blockConfig.basicBlockSprites[(int)_block.breed];
            }

            //Debug.Log("�� ���� ���� �Ϸ�");
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
            //Debug.Log($"Explosion positions : {explosionObj.transform.position.x}, {explosionObj.transform.position.y}");

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

