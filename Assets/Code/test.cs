using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

public class test : MonoBehaviour {
    public EventHandler<byte[]> SourceFileLoadedEventHandler;   // データ取得成功時に送信するイベント

    public Dropdown selectQuiz;

    [SerializeField] string sourceFileName; 

    
    // CSVのデータを入れるリストを入れるリスト
    public static List<List<string[]>> csvDatasList = new List<List<string[]>>();// 追記
    private void Awake ()
    {
        NCMBQuery<NCMBFile> query = NCMBFile.GetQuery ();
        selectQuiz.ClearOptions();

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

                            // csvファイルをstringに変換
                            string csvString = System.Text.Encoding.UTF8.GetString (fileData);
                            // 格納
                            string[] lines = csvString.Replace("\r\n", "\n").Split("\n"[0]);
                            foreach (var line in lines){
                                if (line == "") {continue;}
                                csvDatas.Add(line.Split(','));  // string[]を追加している
                            }
                            csvDatasList.Add(csvDatas);

                            if (selectQuiz)
                            {
                                List<string> options = new List<string>();
                                options.Add(file.FileName + "(" + csvDatas.Count + "問)");
                                selectQuiz.AddOptions(options);
                            }
                            // 書き出し
                            Debug.Log (csvDatas.Count); // 行数
                            Debug.Log (csvDatas[0].Length); // 項目数
                            Debug.Log (csvDatas [1] [1]);   // 2行目2列目
                        }
                    });
                    
                }
            }

        });

        
        //Debug.Log(csvDatasList[0].Count);
        //Debug.Log(csvDatasList[1].Count);
        /*
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
        */
        // 5秒後にshowlistを実行
        //Invoke("showlist", 5);
    }

    public void showlist ()
    {
        Debug.Log("csvDataList == " + csvDatasList.Count);
        Debug.Log("csvDataList == " + csvDatasList[0].Count);
        Debug.Log("csvDataList == " + csvDatasList[1].Count);
        Debug.Log("csvDataList == " + csvDatasList[0][1][1]);

        if (selectQuiz)
        {
            selectQuiz.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < csvDatasList.Count; i++)
            {
                options.Add(csvDatasList[0][i][0]);
            }
            selectQuiz.AddOptions(options);
        }
    }
    
}