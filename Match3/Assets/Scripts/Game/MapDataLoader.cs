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
        string dataAsJson = File.ReadAllText(fileName);                  // fileName 파일에 있는 내용을 "dataAsJson" 변수에 문자열로 저장

        MapData mapData = new MapData();
        mapData = JsonConvert.DeserializeObject<MapData>(dataAsJson);    // 역직렬화로 dataAsJson 변수에 있는 문자열 데이터를 MapData 클래스 인스턴스에 저장

        return mapData;
    }
}
