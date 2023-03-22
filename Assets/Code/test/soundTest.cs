using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;// 追記
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using NCMB;
using System.Linq;

public class soundTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        LoadSound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 音声を生成する関数
    public void LoadSound()
    {
        Debug.Log("LoadSoundを実行");
        NCMBQuery<NCMBFile> query = NCMBFile.GetQuery ();
        //selectQuiz.ClearOptions();
        Hashtable where = new Hashtable();
        where.Add("$regex", ".*\\.mp3$");
        query.WhereEqualTo("fileName", where);

        GameObject soundSource = GameObject.Find("SoundSource");
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
                    file.FetchAsync ((byte[] fileData, NCMBException e) => {
                        if (e != null) {
                            // 取得失敗
                            Debug.Log ( "Source File Load Failed" );
                        } else {
                            // 取得成功
                            Debug.Log ( "Source File Load Succeeded" );
                            Debug.Log (file.FileName);
                    
                            // バイト配列をfloat配列に変換
                            float[] fileDataFloat = new float[fileData.Length / 4];
                            for (int i = 0; i < fileDataFloat.Length; i++)
                            {
                                fileDataFloat[i] = System.BitConverter.ToSingle(fileData, i * 4);
                            }
                            /*  音量注意
                            // AudioClipを作成
                            AudioClip audioClip = AudioClip.Create(file.FileName, fileDataFloat.Length, 1, 44100, false);
                            audioClip.SetData(fileDataFloat, 0);
                            // AudioSourceを作成
                            GameObject sound = new GameObject();
                            sound.transform.parent = soundSource.transform;
                            sound.AddComponent<AudioSource>();
                            sound.GetComponent<AudioSource>().clip = audioClip;

                            // 再生する
                            sound.GetComponent<AudioSource>().Play();
                            

                            // 名前を設定
                            sound.GetComponent<AudioSource>().name = file.FileName.Replace(".mp3", "");
                            // 無効にする
                            //image.gameObject.SetActive(false);
                            */
                        }
                    });
                }
            }
        });
    }
}
