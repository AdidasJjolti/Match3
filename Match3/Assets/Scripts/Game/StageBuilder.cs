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

        public Stage ComposeStage(int row, int col)
        {

            // Stage 객체 생성
            Stage stage = new Stage(this, row, col);

            for(int nRow = 0; nRow < row; nRow++)
            {
                for(int nCol = 0; nCol < col; nCol++)
                {
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }

            return stage;
        }


        //임시로 모든 블럭을 기본 타입, 모든 셀을 노말 타입으로 불러오는 함수 생성

        Block SpawnBlockForStage(int row, int col)
        {
            return new Block(_eBlockType.BASIC);
        }

        Cell SpawnCellForStage(int row, int col)
        {
            return new Cell(_eTileType.NORMAL);
        }

        // StageBuilder 생성
        // Stage를 구성하는 cell, block을 생성하는 ComposeStage를 호출하여 Stage 생성
        public static Stage BuildStage(int nStage, int row, int col)
        {
            StageBuilder stageBuilder = new StageBuilder(0);   // 0번 스테이지 생성
            if(stageBuilder == null)
            {
                Debug.Log("스테이지 빌더가 없음");
            }
            Stage stage = stageBuilder.ComposeStage(9, 9);

            return stage;
        }
    }
}

