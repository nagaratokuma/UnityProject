using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
public class Result : MonoBehaviour
{
    // 結果を表示するText
    public Text resultText;

    // 次の問題に進むボタン
    public Button nextButton;

    public GameObject VotePanel;

    // upperPanel
    public GameObject upperPanel;

    // Start is called before the first frame update
    void Start()
    {
        // 1秒後に結果を表示する
        Invoke("ShowResult", 1f);
    }

    void ShowResult()
    {
        // ルームのカスタムプロパティから正解数を取得する
        int correctCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["CC"];
        // ルームのプレイヤー数を取得する
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // 結果を表示する
        // 不正解者数が一人の時
        if (playerCount - correctCount == 1)
        {
            // 不正解者のIDを取得してルームのカスタムプロパティに格納する
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if ((bool)player.CustomProperties["isCorrect"] == false)
                {
                    //ルームのカスタムプロパティに不正解者のIDを格納する
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VoteAnswer", player.ActorNumber} });
                }
            }
            
            resultText.text = "この中にバカが一人いました。バカを見つけてください。";
            // 2秒後にvoteシーンに遷移する
            Invoke("Vote", 2f);
        }
        else
        {
            // ルームのカスタムプロパティから問題番号を取得する
            int qN = HoldValue.questionNumber;
            int QuizNumInt = (int)PhotonNetwork.CurrentRoom.CustomProperties["QN"];
            // 答えを表示する
            resultText.text = "正解は「<color=yellow>" + ReadCSV.csvDatasList[qN][QuizNumInt-1][2].Replace(Environment.NewLine, "") + "</color>」でした。" + Environment.NewLine;
            resultText.text += "正解したのは" + correctCount + "人でした。";

            // マスタークライアントのみ次の問題に進むボタンを表示する
            if (PhotonNetwork.IsMasterClient)
            {
                nextButton.gameObject.SetActive(true);
            }
            // 2秒後にQuizシーンに遷移する
            //Invoke("GoToQuiz", 2f);
        }
    }

    // 次の問題に進むボタンを押した時に呼ばれる関数
    public void OnClickNextButton()
    {
        // ルームのカスタムプロパティから問題番号を取得する
        int qN = HoldValue.questionNumber;
        int QuizNumInt = (int)PhotonNetwork.CurrentRoom.CustomProperties["QN"];

        // 問題数が最大値を超えていたら
        if (QuizNumInt > HoldValue.MaxQuizNum)
        {
            // GameResultシーンをロード
            PhotonNetwork.LoadLevel("GameResult");
        }
        else 
        {
            // 次の問題のタイプを取得する
            int QuizType = int.Parse(ReadCSV.csvDatasList[qN] [QuizNumInt] [0]);
        
            // 問題のタイプが1の時
            if (QuizType == 1)
            {
                Debug.Log("<color=yellow>Quiz1をロード</color>");
                // Quizシーンをロード
                PhotonNetwork.LoadLevel("Quiz");
            }
            else if (QuizType == 2)
            {
                Debug.Log("<color=yellow>Quiz2をロード</color>");
                // Quiz2シーンをロード
                PhotonNetwork.LoadLevel("Quiz2");
            }
            else if (QuizType == 3)
            {
                Debug.Log("<color=yellow>Quiz3をロード</color>");
                // Quiz3シーンをロード
                PhotonNetwork.LoadLevel("Quiz3");
            }       
            else 
            {
                Debug.Log("問題のタイプが不正です。");
            } 
        }
        
    }

    // プレーヤーのパネルを表示する関数
    public void ShowAllPlayerPanel()
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
            upperPanel.transform.Find(player.ActorNumber.ToString()).gameObject.SetActive(true);
            // VoteButtonを非表示にする
            upperPanel.transform.Find(player.ActorNumber.ToString()).Find("VoteButton").gameObject.SetActive(false);
            // バカを表示するときは解答を表示する
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

    // voteシーンに遷移する
    void Vote()
    {
        PhotonNetwork.LoadLevel("Vote");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
