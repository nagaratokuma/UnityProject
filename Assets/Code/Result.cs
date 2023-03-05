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
            int qN = (int)PhotonNetwork.CurrentRoom.CustomProperties["QD"];
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
        // Quizシーンをロード
        PhotonNetwork.LoadLevel("Quiz");
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
