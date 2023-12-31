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
            //UpdateView(false);    // 타입에 따라 정해진 스프라이트 출력하도록 호출, 초기 생성할 때 false를 인자로 전달
        }


        // Block 객체 참조를 저장
        //internal void SetBlock(Block block)
        //{
        //    _block = block;
        //}

        // 타입이 EMPTY인 경우 스프라이트를 null로 세팅
        // 타입이 ITEM인 경우 listSize가 4이면 세로 매칭인 경우 가로 레이저, 가로 매칭인 경우 세로 레이저로 변경, listSize가 5 이상이면 폭탄으로 변경
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
                    if(blockList[0].blockObj.position.x == blockList[1].blockObj.position.x)        // 세로 매칭
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

            //Debug.Log("블럭 종류 설정 완료");
        }

        // 타입이 EMPTY인 경우 스프라이트를 null로 세팅
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

            //Debug.Log("블럭 종류 설정 완료");
        }

        public void DoActionClear()
        {
            StartCoroutine(CoStartSimpleExplosion(true));
        }

        IEnumerator CoStartSimpleExplosion(bool destroy = true)     // 디폴트 매개 변수로 true 설정, 호출할 때 파라미터를 넣지 않는 경우 true로 설정, 디폴트 매개 변수는 매개 변수 중 뒤쪽에만 배치 가능
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

