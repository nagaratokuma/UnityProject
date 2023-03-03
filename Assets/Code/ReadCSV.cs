using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NCMB;

public class ReadCSV : MonoBehaviour {
    public EventHandler<byte[]> SourceFileLoadedEventHandler;   // データ取得成功時に送信するイベント

    [SerializeField] string sourceFileName; 

    // CSVのデータを入れるリスト
    public static List<string[]> csvDatas = new List<string[]>();// 追記

    private void Start ()
    {
        NCMBFile file = new NCMBFile (sourceFileName);
        file.FetchAsync ((byte[] fileData, NCMBException error) => {
            if (error != null) {
                // 失敗
                Debug.Log ( "Source File Load Failed" );
            } else {
                // 成功
                Debug.Log ( "Source File Load Succeeded" );
                // csvファイルをstringに変換
                string csvString = System.Text.Encoding.UTF8.GetString (fileData);
                // 格納
                string[] lines = csvString.Replace("\r\n", "\n").Split("\n"[0]);
                foreach (var line in lines){
                    if (line == "") {continue;}
                    csvDatas.Add(line.Split(','));  // string[]を追加している
                }

                // 書き出し
                Debug.Log (csvDatas.Count); // 行数
                Debug.Log (csvDatas[0].Length); // 項目数
                Debug.Log (csvDatas [1] [2]);   // 2行目3列目
            }
        });
    }
    
}