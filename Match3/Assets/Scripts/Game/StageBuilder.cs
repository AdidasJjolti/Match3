using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Board;

namespace Match3.Stage
{
    public class StageBuilder
    {
        int _nStage;   // 플레이중인 스테이지 번호


        public StageBuilder(int nStage)
        {
            _nStage = nStage;
        }

        public Stage ComposeStage(int row, int col, G_TileMap2D tilemap2D)
        {
            // Stage 객체 생성
            Stage stage = new Stage(this, row, col);

            for(int nRow = 0; nRow < row; nRow++)
            {
                for(int nCol = 0; nCol < col; nCol++)
                {
                    Debug.Log("블럭 배열 만들게");
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol, tilemap2D);
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol, tilemap2D);
                }
            }

            return stage;
        }


        Block SpawnBlockForStage(int row, int col, G_TileMap2D tilemap2D)
        {
            _eTileType cellType = (_eTileType)Match3.Stage.StageController._data._mapData[row * tilemap2D.GetWidth() + col];
            if (cellType == _eTileType.GRASS || cellType == _eTileType.EMPTY)
            {
                return new Block(_eBlockType.EMPTY);
            }
            else
            {
                return new Block(_eBlockType.BASIC);
            }
            //return new Block(_eBlockType.BASIC);
        }

        // tilemap2D에 있는 데이터를 불러와 셀 생성
        Cell SpawnCellForStage(int row, int col, G_TileMap2D tilemap2D)
        {
            _eTileType type = (_eTileType)Match3.Stage.StageController._data._mapData[row * tilemap2D.GetWidth() + col];
            return new Cell(type);
        }

        // StageBuilder 생성
        // Stage를 구성하는 cell, block을 생성하는 ComposeStage를 호출하여 Stage 생성
        public static Stage BuildStage(int nStage, int row, int col, G_TileMap2D tilemap2D)
        {
            StageBuilder stageBuilder = new StageBuilder(nStage);   // 0번 스테이지 생성
            if(stageBuilder == null)
            {
                Debug.Log("스테이지 빌더가 없음");
            }
            Stage stage = stageBuilder.ComposeStage(row, col, tilemap2D);

            return stage;
        }
    }
}

