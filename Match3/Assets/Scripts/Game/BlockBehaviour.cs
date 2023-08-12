using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;

namespace Match3.Board
{
    public class BlockBehaviour : MonoBehaviour
    {
        Block _block;
        SpriteRenderer _renderer;
        [SerializeField] BlockConfig _blockConfig;

        void Start()
        {
            _block = GetComponent<Block>();
            _renderer = GetComponent<SpriteRenderer>();
            UpdateView(false);    // Ÿ�Կ� ���� ������ ��������Ʈ ����ϵ��� ȣ��, �ʱ� ������ �� false�� ���ڷ� ����
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
                _block.breed = (_eBlockBreed)Random.Range(0, 6);
                _renderer.sprite = _blockConfig.basicBlockSprites[(int)_block.breed];
            }
        }
    }
}

