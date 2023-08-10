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
            UpdateView(false);    // 타입에 따라 정해진 스프라이트 출력하도록 호출, 초기 생성할 때 false를 인자로 전달
        }


        // Block 객체 참조를 저장
        internal void SetBlock(Block block)
        {
            _block = block;
        }

        // 타입이 EMPTY인 경우 스프라이트를 null로 세팅
        public void UpdateView(bool valueChange)
        {
            if(_block.type == _eBlockType.EMPTY)
            {
                _renderer.sprite = null;
            }
        }
    }
}

