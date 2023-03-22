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

public class imgTest : MonoBehaviour
{

    public GameObject imagePrefab;
    public GameObject imagePanel;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadImg", 3f);
        Invoke("showimg", 6f);
    }

    // Update is called once per frame
    void LoadImg()
    {
        NCMBQuery<NCMBFile> query = NCMBFile.GetQuery ();
        //selectQuiz.ClearOptions();
        Hashtable where = new Hashtable();
        where.Add("$regex", ".*\\.png$");
        query.WhereEqualTo("fileName", where);

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
                            
                            // 画像をimagePanelに生成
                            var image = Instantiate(imagePrefab, imagePanel.transform);
                            /*
                            image.GetComponent<Image>().sprite = Sprite.Create(
                                Texture2D.whiteTexture,
                                new Rect(0, 0, 128, 128),
                                new Vector2(0.5f, 0.5f)
                            );
                            */
                            // バイナリデータをテクスチャに変換
                            Texture2D texture = new Texture2D(100, 100);
                            texture.LoadImage(fileData);
                            // テクスチャをスプライトに変換
                            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            // スプライトをimageに設定
                            image.GetComponent<Image>().sprite = sprite;
                            image.GetComponent<Image>().preserveAspect = true;

                            // 名前を設定
                            image.GetComponent<Image>().name = file.FileName.Replace(".png", "");
                            // 無効にする
                            image.GetComponent<Image>().enabled = false;
                        }
                    });
                }
            }
        });
    }

    void showimg() 
    {
        // imagePanelの子要素から名前が2のものを取得
        var image2 = imagePanel.transform.Find("2");
        // 有効にする
        image2.GetComponent<Image>().enabled = true;
    }
}
