using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Board;

namespace Match3.Stage
{
    public class StageBuilder
    {
        int _nStage;   // �÷������� �������� ��ȣ

        public StageBuilder(int nStage)
        {
            _nStage = nStage;
        }

        public Stage ComposeStage(G_TileMap2D tilemap2D)
        {
            if (tilemap2D == null)
            {
                Debug.Log("Tilemap2D Argument Was Null.");
                return null;
            }

            int row = tilemap2D.GetWidth();
            int col = tilemap2D.GetHeight();

            // Stage ��ü ����
            Stage stage = new Stage(this, row, col);

            for (int nRow = 0; nRow < row; nRow++)
            {
                for(int nCol = 0; nCol < col; nCol++)
                {
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol, tilemap2D);
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol, tilemap2D);
                }
            }

            return stage;
        }


        Block SpawnBlockForStage(int row, int col, G_TileMap2D tilemap2D)
        {
            //_eTileType cellType = (_eTileType)Match3.Stage.StageController._data._mapData[row * tilemap2D.GetWidth() + col];
            //if (cellType == _eTileType.GRASS || cellType == _eTileType.EMPTY)
            //{
            //    return new Block(_eBlockType.EMPTY);
            //}
            //else
            //{
            //    return new Block(_eBlockType.BASIC);
            //}

            return tilemap2D.tileMapBlocks[row, col].GetComponent<Block>();
        }

        // tilemap2D�� �ִ� �����͸� �ҷ��� �� ����
        Cell SpawnCellForStage(int row, int col, G_TileMap2D tilemap2D)
        {
            _eTileType type = (_eTileType)Match3.Stage.StageController._data._mapData[row * tilemap2D.GetWidth() + col];
            return new Cell(type);
        }

        // StageBuilder ����
        // Stage�� �����ϴ� cell, block�� �����ϴ� ComposeStage�� ȣ���Ͽ� Stage ����
        public static Stage BuildStage(int nStage, G_TileMap2D tilemap2D)
        {
            StageBuilder stageBuilder = new StageBuilder(nStage);   // 0�� �������� ����
            if(stageBuilder == null)
            {
                Debug.Log("�������� ������ ����");
            }
            Stage stage = stageBuilder.ComposeStage(tilemap2D);
                       
            return stage;
        }
    }
}

