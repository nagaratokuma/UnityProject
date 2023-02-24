using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;// 追記
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class Vote : MonoBehaviourPunCallbacks
{

    private Hashtable PlayerHashtable = new ExitGames.Client.Photon.Hashtable();

    // 投票結果を表示するText
    public Text voteResultText;
    public Text resultText;

    public GameObject VotePanel;

    // upperPanel
    public GameObject upperPanel;

    // 次の問題に進むボタン
    public Button NextButton;

    // プレイヤーごとの投票された数を格納する辞書
    Dictionary<int, int> voteCount = new Dictionary<int, int>();

    public static Vote instance;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        
        // プレイヤーごとの投票された数を初期化する
        foreach (var player in PhotonNetwork.PlayerList)
        {
            voteCount[player.ActorNumber] = 0;
        }
        // 無投票用のkeyとvalueを追加する
        voteCount[0] = 0;
    }

    public void SendPlayerVote(string Votedname)
    {
        Debug.Log("Vote: " + Votedname);
    }
    
    // Voteボタンが押されたときに呼び出される
    public void OnClickVoteButtonMethod()
    {
        // 押されたボタンの名前を取得する
        var buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        // 押されたボタンの名前がNoBakaButtonの場合
        Debug.Log(buttonName);
        if (buttonName == "NoBakaButton")
        {
            // プレイヤーカスタムプロパティに押されたボタンの名前を格納する
            PlayerHashtable["Vote"] = 0;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerHashtable);
        }
        // 押されたボタンの名前がVoteの場合
        else
        {
            // 押されたボタンの親オブジェクトの名前を取得する
            var parentName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.name;
            // プレイヤーカスタムプロパティに押されたボタンの名前をint にしたものを格納する
            PlayerHashtable["Vote"] = int.Parse(parentName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerHashtable);
            Debug.Log(parentName);
        }
        

        /*
        // 押されたボタンの名前から投票先のプレイヤー番号を取得する
        var vote = buttonName;
        // 投票先のプレイヤー番号を送信する
        SendPlayerVote(vote);
        */
    }
    
    // プレイヤーカスタムプロパティ"Vote"が更新されたときに呼び出される
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.ContainsKey("Vote")) return;

        
        // 投票先のプレイヤー番号を取得する
        int vote = (int)changedProps["Vote"];
        // voteCountの値を更新する
        voteCount[vote]++;
        var votedPlayer = PhotonNetwork.PlayerList.First(x => x.ActorNumber == vote);
        Debug.Log(targetPlayer.NickName + "が" + votedPlayer + "に投票しました。");
        // 全員の投票が終わったかどうかを判定する
        if (voteCount.Sum(x => x.Value) == PhotonNetwork.PlayerList.Length)
        {
            // 全員の投票が終わった場合

            // VotePanelを非表示にする
            VotePanel.SetActive(false);

            // resultTextを表示する
            resultText.gameObject.SetActive(true);

            var resultstr = "";

            // 投票結果を表示する
            foreach (var player in PhotonNetwork.PlayerList)
            {
                // 投票された数を取得する
                var count = voteCount[player.ActorNumber];
                // 投票された数を表示する

                resultstr += player.NickName + " : " + count + "票" + "   ";
                Debug.Log(player.NickName + "は" + count + "票");
            }
            // 平和村の投票数を追加する
            resultstr += "インテリ村 : " + voteCount[0] + "票";
            voteResultText.text = resultstr;
            

            // 投票数が最大のプレイヤー番号を取得する
            var maxVote = voteCount.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            // 投票数が最大のプレイヤーを取得する
            var maxVotePlayer = PhotonNetwork.PlayerList.First(x => x.ActorNumber == maxVote);
            // 投票数が最大のプレイヤーの名前を取得する
            var maxVotePlayerName = maxVotePlayer.NickName;
            // 投票数が最大のプレイヤーの名前を表示する
            Debug.Log("<color=blue>" + maxVotePlayerName + "</color>が最も多く投票されました。");

            // 3秒後にShowQuizResult()を呼び出す
            Invoke("ShowQuizResult", 3.0f);
        }
    }

    // NextButtonが押されたときに呼び出される
    public void OnClickNextButtonMethod()
    {
        // Quizシーンをロードする
        PhotonNetwork.LoadLevel("Quiz");
    }

    // クイズの結果を表示する関数
    public void ShowQuizResult()
    {
        // resultTextを非表示にする
        resultText.gameObject.SetActive(false);

        // voteResultTextを非表示にする
        voteResultText.gameObject.SetActive(false);

        // votePanelを表示する
        VotePanel.SetActive(true);
        // votePanelの子オブジェクトlowerを無効にする
        VotePanel.transform.Find("lower").gameObject.SetActive(false);
        
        // upperPanelの子オブジェクトすべてに変更を加える
        var playerNum = 1;
        foreach (Transform child in upperPanel.transform)
        {
            // childのvoteButtonを非表示にする
            child.Find("VoteButton").gameObject.SetActive(false);
            // childのSpeechBaloonを表示する
            child.Find("SpeechBaloon").gameObject.SetActive(true);
            // プレイヤーのカスタムプロパティからiscorrectを取得する
            foreach (var player in PhotonNetwork.PlayerList)
            {
                Debug.Log("IsCorrect == " + player.CustomProperties.ContainsKey("IsCorrect"));
            }
            var iscorrect = PhotonNetwork.PlayerList.First(x => x.ActorNumber == playerNum).CustomProperties["isCorrect"];
            Debug.Log("IsCorrect = " + iscorrect);
            // iscorrectがtrueの場合
            if ((bool)iscorrect)
            {
                // SpeechBaloonの子オブジェクトmaruを表示する
                child.Find("SpeechBaloon").Find("maru").gameObject.SetActive(true);
            }
            // iscorrectがfalseの場合
            else
            {
                // SpeechBaloonの子オブジェクトbatuを表示する
                child.Find("SpeechBaloon").Find("batu").gameObject.SetActive(true);
            }
        
            playerNum += 1;
        }


        // マスタークライアントの場合
        if (PhotonNetwork.IsMasterClient)
        {
            // NextButtonを表示する
            NextButton.gameObject.SetActive(true);
        }
    }
}
