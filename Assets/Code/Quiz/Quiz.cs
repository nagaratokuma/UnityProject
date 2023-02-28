using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;// 追記
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class Quiz : MonoBehaviourPunCallbacks {
    public static Quiz instance;
    public TextAsset csvFile;// GUIでcsvファイルを割当

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

    // 問題番号を格納する変数
    public static int questionNumber = 0;

    // CSVのデータを入れるリスト
    List<string[]> csvDatas = new List<string[]>();// 追記

    // Playerの正誤を格納する辞書
    Dictionary<string, bool> playerAnswer = new Dictionary<string, bool>();
    
    private Hashtable RoomHashtable = new ExitGames.Client.Photon.Hashtable();

    void Awake(){
        // PhotonNetwork.AutomaticallySyncScene を有効にするとマスタークライアントがシーンをロードすると他のクライアントも同じシーンをロードするようになる
        PhotonNetwork.AutomaticallySyncScene = true;

        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        // 初期化
        isSent = false;
        isCorrect = false;
        questionText.text = "";

        // QuizNumberを表示
        QuizNumber.text = "第" + (questionNumber + 1) + "問";

        // 格納
        string[] lines = csvFile.text.Replace("\r\n", "\n").Split("\n"[0]);
        foreach (var line in lines){
            if (line == "") {continue;}
            csvDatas.Add(line.Split(','));  // string[]を追加している
        }


        // 1秒後に問題文を表示する
        Invoke("ShowQuestion", 1.0f);
    }

    // 問題文を表示する関数
    public void ShowQuestion(){
        // ルームのカスタムプロパティから問題番号を取得
        // ルームのカスタムプロパティにQDがない場合は0を代入
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("QD") == false) {
            questionNumber = 0;
        } else {
            questionNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties["QD"];
        }
        Debug.Log("QD: " + questionNumber);
        // 問題文を表示
        questionText.text = csvDatas [questionNumber] [1];
    }

    // AnswerButtonを押したときに呼ばれる関数
    public void AnswerButton(){
        // AnswerInputFieldのテキストを取得
        string answer = answerInputField.text;

        // 答え合わせ
        if (csvDatas [questionNumber] [2] == answer) {
            isCorrect = true;
        } else {
            isCorrect = false;
        }
        
        // 正誤判定を送信
        SendPlayerAnswer(isCorrect);   
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
                RoomHashtable.Add("CC", correctCount);
                PhotonNetwork.CurrentRoom.SetCustomProperties(RoomHashtable);

                // 問題番号を更新
                questionNumber++;

                // マスタークライアントのみが次のシーンをロードする
                if (PhotonNetwork.IsMasterClient)
                {
                    // 問題番号をルームのカスタムプロパティに保存
                    RoomHashtable.Add("QD", questionNumber);
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
    public void SendPlayerAnswer(bool isCorrect){
        // AnswerButtonのボタンを押せなくする
        answerButton.interactable = false;

        Debug.Log("プレイヤーカスタムプロパティを送信");
        // カスタムプロパティを送信
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("isCorrect", isCorrect);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        isSent = true;
    }
}