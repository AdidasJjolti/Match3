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
        MapData mapData = _tilemap2D.GetMapData();    // tilemap2D에 저장횐 맵 데이터 불러오기
        string fileName = _inputFileName.text;        // 인풋필드에 입력한 텍스트로 파일명 지정

        if(fileName.Contains(".json") == false)       // 파일명에 .json 확장자가 없으면 추가
        {
            fileName += ".json";
        }

        fileName = Path.Combine("Assets/MapData/", fileName);  // 스트링 합치기

        // mapData에 있는 내용을 toJson에 스트링 형태로 저장, Formatting.Indented로 들여쓰기 추가
        string toJson = JsonConvert.SerializeObject(mapData, Formatting.Indented);
        File.WriteAllText(fileName, toJson);
    }
}
