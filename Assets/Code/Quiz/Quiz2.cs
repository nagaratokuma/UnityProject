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

public class Quiz2 : MonoBehaviourPunCallbacks {
    public static Quiz2 instance;

    public ChangeImage changeImage;

    // AnswerInputField
    public InputField answerInputField;

    // 問題文を表示するText
    public Text questionText;

    // 問題番号を表示するText
    public Text QuizNumber;

    // 解答ボタン
    public Button answerButton;

    // 正誤判定を格納する変数
    bool isCorrect = false;

    // 正誤判定を送信したかどうかを格納する変数
    public bool isSent = false;

    // csvから参照するのに使う問題番号を格納する変数
    private int questionNumber = 1;

    // 問題番号を格納する変数
    //public static int HoldValue.QuizNumInt = 1;

    // Playerの正誤を格納する辞書
    Dictionary<string, bool> playerAnswer = new Dictionary<string, bool>();
    
    // 解答を格納する変数
    public string answer;

    // 画像を表示するパネル
    //public GameObject imagePanel;

    // 画像を表示するプレハブ
    public GameObject imagePrefab;

    // LoadIMGが実行されたかどうか
    public static bool isLoadIMG = false;

    void Awake(){
        // PhotonNetwork.AutomaticallySyncScene を有効にするとマスタークライアントがシーンをロードすると他のクライアントも同じシーンをロードするようになる
        PhotonNetwork.AutomaticallySyncScene = true;

        // プレイヤーカスタムプロパティから正解数を取得
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("QC") == false) {
            Debug.Log("QCがありません");
        }
        else 
        {
            Debug.Log("正解数: " + PhotonNetwork.LocalPlayer.CustomProperties["QC"]);
        }

        // マスタークライアントの場合
        if (PhotonNetwork.IsMasterClient)
        {
            // 全問解答したらGameResultシーンに遷移
            if (HoldValue.QuizNumInt > HoldValue.MaxQuizNum)
            {
                // GameResultシーンに遷移
                PhotonNetwork.LoadLevel("GameResult");
            }     
        }

        if(instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        // 初期化
        isSent = false;
        isCorrect = false;
        questionText.text = "";

        // QuizNumberを表示
        QuizNumber.text = "第" + (HoldValue.QuizNumInt) + "問";

        // LoadIMGが実行されていない場合
        if (isLoadIMG == false) {
            // LoadIMGを実行
            LoadImg();
            isLoadIMG = true;
            
            
        }
        else
        {
            // 1秒後に問題文を表示する
            Invoke("ShowQuestion", 2.0f);
        }
        
    }

    // 問題文を表示する関数
    public void ShowQuestion(){

        // ルームのカスタムプロパティから問題を取得
        // ルームのカスタムプロパティにQDがない場合は0を代入
        Hashtable RoomHashtable = new ExitGames.Client.Photon.Hashtable();
        questionNumber = HoldValue.questionNumber;

        // ルームのカスタムプロパティから問題番号を取得
        // ルームのカスタムプロパティにQNがない場合は0を代入
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("QN") == false) {
            HoldValue.QuizNumInt = 1;
            RoomHashtable.Add("QN", HoldValue.QuizNumInt);
            PhotonNetwork.CurrentRoom.SetCustomProperties(RoomHashtable);
        } else {
            HoldValue.QuizNumInt = (int)PhotonNetwork.CurrentRoom.CustomProperties["QN"];
        }
        Debug.Log("QD: " + questionNumber);

        // 問題文を表示
        questionText.text = ReadCSV.csvDatasList[questionNumber] [HoldValue.QuizNumInt] [1];
  
        GameObject imagePanel = GameObject.Find("ImagePanel");

        // 名前がHoldValue.QuizNumIntであるimagePanelの子オブジェクトがあるかどうかを確認
        if (imagePanel.transform.Find(HoldValue.QuizNumInt.ToString()) != null) {
            // 名前がHoldValue.QuizNumIntであるimagePanelの子オブジェクトを有効にする
            
            GameObject image = imagePanel.transform.Find(HoldValue.QuizNumInt.ToString()).gameObject;
            //GameObject image = imagePanel.transform.Find(HoldValue.QuizNumInt.ToString()).gameObject;
            image.gameObject.SetActive(true);
            // 画像を表示
            Debug.Log("画像を表示");
            
        }
        else
        {
            Debug.Log("オブジェクトがありません");
            Debug.Log("<color=red>" + HoldValue.QuizNumInt.ToString() + "</color>");
        }

    }

    // AnswerButtonを押したときに呼ばれる関数
    public void AnswerButton(){
        // AnswerInputFieldのテキストを取得
       //answer = answerInputField.text;

        // 答え合わせ
        if (ReadCSV.csvDatasList [questionNumber] [HoldValue.QuizNumInt] [2] == answer) {
            isCorrect = true;
        } else {
            isCorrect = false;
        }
        // 解答を送信

        // 正誤判定を送信
        SendPlayerAnswer(isCorrect, answer);   
    }

    // answerInputFieldの値が変更されたときに呼ばれる関数
    public void OnValueChanged(){
        // 解答を取得
        answer = answerInputField.text;
    }

    // プレイヤーカスタムプロパティを受け取ったときに呼ばれる関数
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isCorrect"))
        {
            // targetPlyaerのplayerPanelを取得する
            var playerPanel = GameObject.Find(targetPlayer.ActorNumber.ToString());
            if (playerPanel != null) 
            {
                // playerPanelの子オブジェクトのSpeechBalloonを取得する
                var speechBalloon = playerPanel.transform.Find("SpeechBaloon").gameObject;
                // speechBaloonの子オブジェクトのAnsweredを有効にする
                speechBalloon.transform.Find("Answered").gameObject.SetActive(true);
                // speechBalonnの子オブジェクトのAnsweringAnimを無効にする
                speechBalloon.transform.Find("AnsweringAnim").gameObject.SetActive(false);
                
            }
            else
            {
                Debug.Log("<color=red>playerPanelが見つかりませんでした。</color>");
            }
            // playerAnswerに追加
            playerAnswer.Add(targetPlayer.NickName, (bool)changedProps["isCorrect"]);
            //Debug.Log("Player: " + targetPlayer.NickName);
            //Debug.Log("isCorrect: " + changedProps["isCorrect"]);

            if (playerAnswer.Count == PhotonNetwork.PlayerList.Length)
            {
                GameObject imagePanel = GameObject.Find("ImagePanel");
                GameObject image = imagePanel.transform.Find(HoldValue.QuizNumInt.ToString()).gameObject;
                image.gameObject.SetActive(false);
                // 全員の回答が揃ったらここで結果を表示する

                foreach(KeyValuePair<string, bool> pair in playerAnswer)
                {
                    Debug.Log("Player: " + pair.Key + " isCorrect: " + pair.Value);
                }
                // 正解した人数をカウントする変数
                int correctCount = 0;
                foreach(KeyValuePair<string, bool> pair in playerAnswer)
                {
                    if (pair.Value == true)
                    {
                        correctCount++;
                    }
                }
                //Debug.Log("正解した人数: " + correctCount);
                
                // 正解した人数をルームのカスタムプロパティに保存
                Hashtable RoomHashtable = new ExitGames.Client.Photon.Hashtable();
                RoomHashtable.Add("CC", correctCount);
                PhotonNetwork.CurrentRoom.SetCustomProperties(RoomHashtable);

                // 問題番号を更新
                questionNumber++;
                HoldValue.QuizNumInt++;

                // マスタークライアントのみが次のシーンをロードする
                if (PhotonNetwork.IsMasterClient)
                {
                    // 問題番号をルームのカスタムプロパティに保存
                    RoomHashtable.Add("QN", HoldValue.QuizNumInt);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(RoomHashtable);

                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        Debug.Log("IsCorrectが設定されているか" + player.CustomProperties.ContainsKey("isCorrect"));
                        Debug.Log(player.NickName);
                    }
                    // 次のシーンをロード
                    PhotonNetwork.LoadLevel("Result");
                }
            }
        }
        
    }

    // タイマーが制限時間を超えたら呼ばれる関数
    // プレイヤーカスタムプロパティを送信する関数（引数: 正誤判定）
    public void SendPlayerAnswer(bool isCorrect, string Answer){
        // AnswerButtonのボタンを押せなくする
        answerButton.interactable = false;

        var hashtable = new ExitGames.Client.Photon.Hashtable();

        // 解答を送信
        hashtable.Add("Answer", Answer);
        
        Debug.Log("プレイヤーカスタムプロパティを送信");
        // カスタムプロパティを送信
        
        // 正誤判定を送信
        hashtable.Add("isCorrect", isCorrect);
        // 正解の場合
        if (isCorrect == true)
        {
            // カスタムプロパティに正解数を保存
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("QC") == false)
            {
                hashtable.Add("QC", 1);
            }
            else
            {
                hashtable.Add("QC", (int)PhotonNetwork.LocalPlayer.CustomProperties["QC"] + 1);
            }
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        isSent = true;
    }

    // 画像を生成する関数
    public void LoadImg()
    {
        Debug.Log("LoadImgを実行");
        NCMBQuery<NCMBFile> query = NCMBFile.GetQuery ();
        //selectQuiz.ClearOptions();
        Hashtable where = new Hashtable();
        where.Add("$regex", ".*\\.png$");
        query.WhereEqualTo("fileName", where);

        GameObject imagePanel = GameObject.Find("ImagePanel");
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
                            image.gameObject.SetActive(false);
                        }
                    });
                }
            }
        });

        // 3秒後に問題文を表示する
        Invoke("ShowQuestion", 3.0f);
    }
}