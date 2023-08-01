using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class MapDataSave : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputFileName;
    [SerializeField] Tilemap2D _tilemap2D;
    void Awake()
    {
        _inputFileName.text = "NoName.json";
    }

    public void Save()
    {
        MapData mapData = _tilemap2D.GetMapData();    // tilemap2D�� ����Ⱥ �� ������ �ҷ�����
        string fileName = _inputFileName.text;        // ��ǲ�ʵ忡 �Է��� �ؽ�Ʈ�� ���ϸ� ����

        if(fileName.Contains(".json") == false)       // ���ϸ� .json Ȯ���ڰ� ������ �߰�
        {
            fileName += ".json";
        }

        fileName = Path.Combine("Assets/MapData/", fileName);  // ��Ʈ�� ��ġ��

        // mapData�� �ִ� ������ toJson�� ��Ʈ�� ���·� ����, Formatting.Indented�� �鿩���� �߰�
        string toJson = JsonConvert.SerializeObject(mapData, Formatting.Indented);
        File.WriteAllText(fileName, toJson);
    }
}
