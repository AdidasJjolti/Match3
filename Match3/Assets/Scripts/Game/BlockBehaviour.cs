using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class BlockBehaviour : MonoBehaviour
    {
        Block _block;
        SpriteRenderer _renderer;

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            UpdateView(false);    // Ÿ�Կ� ���� ������ ��������Ʈ ����ϵ��� ȣ��, �ʱ� ������ �� false�� ���ڷ� ����
        }


        // Block ��ü ������ ����
        internal void SetBlock(Block block)
        {
            _block = block;
        }

        // Ÿ���� EMPTY�� ��� ��������Ʈ�� null�� ����
        public void UpdateView(bool valueChange)
        {
            if(_block.type == _eBlockType.EMPTY)
            {
                _renderer.sprite = null;
            }
        }
    }
}

