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

        public Stage ComposeStage(int row, int col)
        {

            // Stage ��ü ����
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


        //�ӽ÷� ��� ���� �⺻ Ÿ��, ��� ���� �븻 Ÿ������ �ҷ����� �Լ� ����

        Block SpawnBlockForStage(int row, int col)
        {
            return new Block(_eBlockType.BASIC);
        }

        Cell SpawnCellForStage(int row, int col)
        {
            return new Cell(_eTileType.NORMAL);
        }

        // StageBuilder ����
        // Stage�� �����ϴ� cell, block�� �����ϴ� ComposeStage�� ȣ���Ͽ� Stage ����
        public static Stage BuildStage(int nStage, int row, int col)
        {
            StageBuilder stageBuilder = new StageBuilder(0);   // 0�� �������� ����
            if(stageBuilder == null)
            {
                Debug.Log("�������� ������ ����");
            }
            Stage stage = stageBuilder.ComposeStage(9, 9);

            return stage;
        }
    }
}

