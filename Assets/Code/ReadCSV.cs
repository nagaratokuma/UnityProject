using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

public class ReadCSV : MonoBehaviour {
    public EventHandler<byte[]> SourceFileLoadedEventHandler;   // データ取得成功時に送信するイベント

    public Dropdown selectQuiz;
    
    // CSVのデータを入れるリストを入れるリスト
    public static List<List<string[]>> csvDatasList = new List<List<string[]>>();// 追記
    private void Awake ()
    {
        Invoke("GetQuiz", 1.0f);
    }

    public void setDropdown()
    {
        selectQuiz.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < csvDatasList.Count; i++)
        {
            options.Add(csvDatasList[i][0][0] + "(" + (csvDatasList[i].Count - 1) + "問)");
        }
        selectQuiz.AddOptions(options);
    }
    
    public void GetQuiz()
    {
        NCMBQuery<NCMBFile> query = NCMBFile.GetQuery ();
        //selectQuiz.ClearOptions();

        //query.WhereContainedInArray("fileName", new List<string> { "Quiz.csv"});

        query.WhereNotEqualTo("fileName", "hogehoge");
        query.FindAsync ((List<NCMBFile> objList, NCMBException error) => {
            if (error != null) {
                // 検索失敗
                Debug.Log ( "Source File Load Failed" );
            } else {
                // 検索成功
                Debug.Log ( "Source File Load Succeeded" );
                Debug.Log (objList.Count);
                foreach (NCMBFile file in objList) {
                    // CSVのデータを入れるリスト
                    List<string[]> csvDatas = new List<string[]>();// 追記

                    file.FetchAsync ((byte[] fileData, NCMBException e) => {
                        if (e != null) {
                            // 取得失敗
                            Debug.Log ( "Source File Load Failed" );
                        } else {
                            // 取得成功
                            Debug.Log ( "Source File Load Succeeded" );
                            Debug.Log (file.FileName);
                            
                            // csvDatasにファイル名を入れる
                            csvDatas.Add(new string[]{file.FileName});
                            // csvファイルをstringに変換
                            string csvString = System.Text.Encoding.UTF8.GetString (fileData);
                            // 格納
                            string[] lines = csvString.Replace("\r\n", "\n").Split("\n"[0]);
                            foreach (var line in lines){
                                if (line == "") {continue;}
                                csvDatas.Add(line.Split(','));  // string[]を追加している
                            }
                            csvDatasList.Add(csvDatas);
                            /*
                            if (selectQuiz)
                            {
                                List<string> options = new List<string>();
                                options.Add(file.FileName + "(" + csvDatas.Count + "問)");
                                selectQuiz.AddOptions(options);
                            }
                            */
                            // 書き出し
                            Debug.Log (csvDatas.Count); // 行数
                            Debug.Log (csvDatas[0].Length); // 項目数
                            Debug.Log (csvDatas [1] [1]);   // 2行目2列目
                        }
                    });
                    
                }
            }

        });

        Invoke("setDropdown", 3.0f);
    }
}