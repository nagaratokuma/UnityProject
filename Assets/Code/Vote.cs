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

    public Button NoBakaButton;
    public GameObject VotePanel;

    // upperPanel
    public GameObject upperPanel;

    public Text BakaResultText;

    // 次の問題に進むボタン
    public Button NextButton;

    // 「この中にバカはいませんでした」Text
    public Text NoBakaText;

    // 「が処刑されました」or 「インテリ村が最も多く投票されました。」Text
    public Text VotedOrNoBaka;

    // 「」
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
        
        // VoteButtonを押せなくする
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerPanel = upperPanel.transform.Find(player.ActorNumber.ToString());
            playerPanel.Find("VoteButton").GetComponent<Button>().interactable = false;
        }
        // NoBakaButtonを押せなくする
        NoBakaButton.interactable = false;
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
        if (changedProps.ContainsKey("VC") || changedProps.ContainsKey("DC")) 
        {
            Debug.Log("Vote: " + targetPlayer.NickName + "のカスタムプロパティが更新されました。");
        }
        if (!changedProps.ContainsKey("Vote")) return;

        // ルームのカスタムプロパティから正解数を取得する
        int correctCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["CC"];

        // 投票先のプレイヤー番号を取得する
        int vote = (int)changedProps["Vote"];
        // voteCountの値を更新する
        voteCount[vote]++;
        if (vote != 0)
        {
            var votedPlayer = PhotonNetwork.PlayerList.First(x => x.ActorNumber == vote);
            Debug.Log(targetPlayer.NickName + "が" + votedPlayer + "に投票しました。");
        }
        

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
            // インテリ村の投票数を追加する
            resultstr += "インテリ村 : " + voteCount[0] + "票";
            voteResultText.text = resultstr;
        
            // 投票数が最大のプレイヤー番号を取得する
            var maxVote = voteCount.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            //  最大の投票数がほかにもあった場合
            if (voteCount.Count(x => x.Value == voteCount[maxVote]) > 1)
            {
                // マスタークラインとのみ処理を行う
                if (!PhotonNetwork.IsMasterClient) return;
                Debug.Log("最大の投票数が複数あります。");
                //　ランダムにプレイヤーを選ぶ
                // voteCountの値がvoteCount[maxVote]と同じキーを取得する
                var duplicated = voteCount.GroupBy( c => c.Value ).Where( g => g.Key == voteCount[maxVote] ).SelectMany( g => g.Select( c => c.Key ) ).ToList();
                Debug.Log("duplicated: " + duplicated);
                var random = new System.Random();
                var randomSelect = duplicated[random.Next(0, duplicated.Count())];
                Debug.Log("randomSelect: " + randomSelect);
                // ルームのカスタムプロパティにランダムに選んだプレイヤー番号を格納する
                var RoomHashtable = new Hashtable();
                RoomHashtable["RS"] = randomSelect;
                PhotonNetwork.CurrentRoom.SetCustomProperties(RoomHashtable);
                return;
            }
            
            // インテリ村が最も多く投票された場合
            if (maxVote == 0)
            {
                // VotedOrNoBakaTextを変更する
                VotedOrNoBaka.text = "インテリ村が最も多く投票されました。";
                // VotedOrNoBakaTextを表示する
                VotedOrNoBaka.gameObject.SetActive(true);
                
                // 投票の正誤を判定してポイントを加算する
                JudgeVoteResult(maxVote);

                // 3秒後にShowBakaResultText()を呼び出す
                Invoke("ShowBakaResultText", 5.0f);
            }
            else 
            {
                // 処刑者が一人選ばれる場合
                // 投票数が最大のプレイヤーを取得する
                var maxVotePlayer = PhotonNetwork.PlayerList.First(x => x.ActorNumber == maxVote);
                // 投票数が最大のプレイヤーの名前を取得する
                var maxVotePlayerName = maxVotePlayer.NickName;
                // 投票数が最大のプレイヤーの名前を表示する
                Debug.Log("<color=blue>" + maxVotePlayerName + "</color>が最も多く投票されました。");

                // 投票の正誤を判定してポイントを加算する
                JudgeVoteResult(maxVote);

                // VotedOrNoBakaTextを表示する
                VotedOrNoBaka.gameObject.SetActive(true);
                // 最も多く投票されたプレイヤーのPanelを表示する
                ShowOnePlayerPanel(maxVotePlayer.ActorNumber, false);
            }
            
            // 3秒後にShowBakaResultText()を呼び出す
            Invoke("ShowBakaResultText", 5.0f);
        }
    }

    // ルームのカスタムプロパティが更新されたときに呼び出される
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // ルームのカスタムプロパティから処刑者を取得する
        if (propertiesThatChanged.ContainsKey("RS"))
        {
            var randomSelected = (int)propertiesThatChanged["RS"];
            // 処刑者を表示する
            Debug.Log("処刑者: " + randomSelected);
            
            // randomSelectedが0の場合
            if (randomSelected == 0)
            {
                // VotedOrNoBakaTextを変更する
                VotedOrNoBaka.text = "インテリ村が最も多く投票されました。";
                // VotedOrNoBakaTextを表示する
                VotedOrNoBaka.gameObject.SetActive(true);
            }
            else 
            {
                // VotedOrNoBakaTextを表示する
                VotedOrNoBaka.text = "が処刑されました。";
                VotedOrNoBaka.gameObject.SetActive(true);
                // 最も多く投票されたプレイヤーのPanelを表示する
                ShowOnePlayerPanel(randomSelected, false);
            }

            // 投票の正誤を判定してポイントを加算する
            JudgeVoteResult(randomSelected);

            // 3秒後にShowBakaResultText()を呼び出す
            Invoke("ShowBakaResultText", 5.0f);
        }
    }

    // NextButtonが押されたときに呼び出される
    public void OnClickNextButtonMethod()
    {
        // Quizシーンをロードする
        PhotonNetwork.LoadLevel("Quiz");
    }

    public void ShowBakaResultText()
    {
        // VotedOrNoBakaTextを非表示する
        VotedOrNoBaka.gameObject.SetActive(false);

        // resultTextを非表示にする
        resultText.gameObject.SetActive(false);
        // voteResultTextを非表示にする
        voteResultText.gameObject.SetActive(false);

        // VotePanelを非表示にする
        VotePanel.SetActive(false);
        
        // クイズの問題番号を取得する
        var qN = (int)PhotonNetwork.CurrentRoom.CustomProperties["QD"];
        // BakaResultTextを設定する
        BakaResultText.text = "正解は「<color=blue>" + Quiz.csvDatas[qN-1][2].Replace(Environment.NewLine, "") + "</color>」です。" + Environment.NewLine + "バカの正体は...";
        // BakaResultTextを表示する
        BakaResultText.gameObject.SetActive(true);
        // ３秒後にShowQuizResult()を呼び出す
        Invoke("ShowQuizResult", 3.0f);
    }
    // クイズの結果を表示する関数
    public void ShowQuizResult()
    {
        
        

        // ルームのカスタムプロパティから正解者数を取得する
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("CC"))
        {
            var answerCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["CC"];
            // 全員正解の場合
            if (answerCount == PhotonNetwork.PlayerList.Length)
            {
                // NoBakaTextを表示する
                NoBakaText.gameObject.SetActive(true); // 「この村にバカはいませんでした。」
            }
            else
            {
                // バカが一人いた場合
                
                // IsCorrectがfalseのプレイヤーを取得する
                var wrongPlayer = PhotonNetwork.PlayerList.First(x => (bool)x.CustomProperties["isCorrect"] == false);
                Debug.Log(wrongPlayer.NickName + "が間違えました。");
                
                // 間違えたプレイヤー以外のPanelを非表示にする
                ShowOnePlayerPanel(wrongPlayer.ActorNumber, true);
            }
        }
        else
        {
            Debug.Log("正解者数が取得できませんでした。");
        }

        // upperPanelの子オブジェクトすべてに変更を加える
        var playerNum = 1;
        


        // マスタークライアントの場合
        if (PhotonNetwork.IsMasterClient)
        {
            // NextButtonを表示する
            NextButton.gameObject.SetActive(true);
        }
    }

    // 入力されたプレーヤーのパネルのみを表示する関数
    public void ShowOnePlayerPanel(int playerNum, bool isBaka)
    {
        // upperPanelの子オブジェクトを全て取得する
        var upperPanel = VotePanel.transform.Find("upper").gameObject;
        // votePanelを表示する
        VotePanel.SetActive(true);
        // votePanelの子オブジェクトlowerを無効にする
        VotePanel.transform.Find("lower").gameObject.SetActive(false);
        // IDがplayerNumのオブジェクト以外を非表示にする
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != playerNum)
            {
                upperPanel.transform.Find(player.ActorNumber.ToString()).gameObject.SetActive(false);
            }
            else
            {
                upperPanel.transform.Find(player.ActorNumber.ToString()).gameObject.SetActive(true);
                // VoteButtonを非表示にする
                upperPanel.transform.Find(player.ActorNumber.ToString()).Find("VoteButton").gameObject.SetActive(false);
                // バカを表示するときは解答を表示する
                if (isBaka == true)
                {
                    if (player.CustomProperties.ContainsKey("Answer") == false)
                    {
                        // AnswerTextを設定する
                        upperPanel.transform.Find(player.ActorNumber.ToString()).Find("Answer").GetComponent<Text>().text = "解答なし";
                    }
                    // AnswerTextを設定する
                    upperPanel.transform.Find(player.ActorNumber.ToString()).Find("Answer").GetComponent<Text>().text = "「" + player.CustomProperties["Answer"].ToString() + "」";
                    // AnswerTextを表示する
                    upperPanel.transform.Find(player.ActorNumber.ToString()).Find("Answer").gameObject.SetActive(true);
                }
                
            }
        }
    }

    // プレイヤーの投票の結果を判定する関数
    public void JudgeVoteResult(int MaxVote)
    {
        // LocalPlayerの投票先が最大の投票数かつ、あっていた場合
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Vote"] == MaxVote && (int)PhotonNetwork.CurrentRoom.CustomProperties["VoteAnswer"] == MaxVote)
        {
            // プレイヤーのカスタムプロパティのVCを更新する
            IncrementPlayerCustomProperties("VC");
        }
        
        // LocalPlayerがバカで処刑されなかった場合
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isCorrect"] == false && (int)PhotonNetwork.LocalPlayer.ActorNumber != MaxVote)
        {
            // プレイヤーのカスタムプロパティのDCを更新する
            IncrementPlayerCustomProperties("DC");
        }
    }

    // プレイヤーのカスタムプロパティを更新する関数
    public void IncrementPlayerCustomProperties(string key)
    {
        Debug.Log("IncrementPlayerCustomProperties  Key == " + key);
        // プレイヤーのカスタムプロパティを更新する
        var PlayerHashtable = new Hashtable();
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key))
        {
            PlayerHashtable[key] = 1;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerHashtable);
            return;
        }
        var preValue = (int)PhotonNetwork.LocalPlayer.CustomProperties[key];
        PlayerHashtable[key] = preValue + 1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerHashtable);
    }

    
}
