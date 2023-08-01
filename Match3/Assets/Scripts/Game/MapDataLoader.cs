using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class MapDataLoader
{
    public MapData Load(string fileName)
    {
        if(fileName.Contains(".json") == false)
        {
            fileName += ".json";
        }

        fileName = Path.Combine("Assets/MapData/", fileName);
        string dataAsJson = File.ReadAllText(fileName);                  // fileName ���Ͽ� �ִ� ������ "dataAsJson" ������ ���ڿ��� ����

        MapData mapData = new MapData();
        mapData = JsonConvert.DeserializeObject<MapData>(dataAsJson);    // ������ȭ�� dataAsJson ������ �ִ� ���ڿ� �����͸� MapData Ŭ���� �ν��Ͻ��� ����

        return mapData;
    }
}
