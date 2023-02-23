using UnityEngine;
using System.Collections;
using System.Collections.Generic;// 追記

public class ReadCSV : MonoBehaviour {
    public TextAsset csvFile;// GUIでcsvファイルを割当

    // CSVのデータを入れるリスト
    List<string[]> csvDatas = new List<string[]>();// 追記

    // Use this for initialization
    void Start () {

        // 格納
        string[] lines = csvFile.text.Replace("\r\n", "\n").Split("\n"[0]);
        foreach (var line in lines){
            if (line == "") {continue;}
            csvDatas.Add(line.Split(','));  // string[]を追加している
        }

        // 書き出し
        Debug.Log (csvDatas.Count); // 行数
        Debug.Log (csvDatas[0].Length); // 項目数
        Debug.Log (csvDatas [1] [2]);   // 2行目3列目
    }

    
}