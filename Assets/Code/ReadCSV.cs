using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NCMB;
using UnityEngine.UI;
using System.Linq;

public class ReadCSV : MonoBehaviour {
    public EventHandler<byte[]> SourceFileLoadedEventHandler;   // データ取得成功時に送信するイベント

    public Dropdown selectQuiz;
    
    // CSVのデータを入れるリストを入れる辞書
    public static Dictionary<string, List<string[]>> csvDatasDict = new Dictionary<string, List<string[]>>();// 追記

    // 画像のリストを入れる辞書
    public static Dictionary<string, List<Texture2D>> pngDatasDict = new Dictionary<string, List<Texture2D>>();// 追記
    
    public static List<List<string[]>> csvDatasList = new List<List<string[]>>();// 追記
    private void Awake ()
    {
        Invoke("GetQuiz", 1.0f);
    }

    public void setDropdown()
    {
        selectQuiz.ClearOptions();
        List<string> options = new List<string>();
        foreach (var quiz in csvDatasDict)
        {
            options.Add(quiz.Key + "(" + (quiz.Value.Count - 1) + "問)");
            csvDatasList.Add(quiz.Value);
        }
        selectQuiz.AddOptions(options);
    }
    
    
    public void GetQuiz()
    {
        NCMBQuery<NCMBFile> query = NCMBFile.GetQuery ();
        //selectQuiz.ClearOptions();
        Hashtable where = new Hashtable();
        where.Add("$regex", ".*\\.csv$");
        query.WhereEqualTo("fileName", where);
        //query.WhereContainedInArray("fileName", new List<string> { "test...."});

        //query.WhereNotEqualTo("fileName", "hogehoge");
        query.FindAsync ((List<NCMBFile> objList, NCMBException error) => {
            // 検索結果をファイル名でソート
            objList = objList.OrderBy(x => x.FileName).ToList();
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
                            csvDatasDict.Add(file.FileName, csvDatas);
                            /*
                            if (selectQuiz)
                            {
                                List<string> options = new List<string>();
                                options.Add(file.FileName + "(" + csvDatas.Count + "問)");
                                selectQuiz.AddOptions(options);
                            }
                            */
                            // 書き出し
                            //Debug.Log (csvDatas.Count); // 行数
                            //Debug.Log (csvDatas[0].Length); // 項目数
                            //Debug.Log (csvDatas [1] [1]);   // 2行目2列目
                        }
                    });
                    
                }
            }

        });
        // csvDatasDictをkeyでソート
        csvDatasDict.OrderBy(x => x.Key);
        
        Invoke("setDropdown", 3.0f);
    }
}