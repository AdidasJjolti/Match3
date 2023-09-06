using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Board
{
    public class BlockFactory
    {
        public static Block SpawnBlock(_eBlockType blockType)
        {
            Block block = new Block(blockType);

            //Set Breed
            if (blockType == _eBlockType.BASIC)
            {
                block.breed = (_eBlockBreed)Random.Range(0, (int)_eBlockBreed.MAX);
            }
            else if(blockType == _eBlockType.EMPTY)
            {
                block.breed = _eBlockBreed.NONE;
            }

            return block;
        }
    }
}

