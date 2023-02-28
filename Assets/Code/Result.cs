using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    // 結果を表示するText
    public Text resultText;

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
        // 全員正解だった場合
        else if (playerCount == correctCount)
        {
            // ルームのカスタムプロパティに投票の正解を格納する
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VoteAnswer", 0 } });
            resultText.text = "この中にバカが一人いました。バカを見つけてください。";
            // 2秒後にQuizシーンに遷移する
            Invoke("Vote", 2f);
        }
        else
        {
            resultText.text = "正解したのは" + correctCount + "人でした。次の問題に進みます。";
            // 2秒後にQuizシーンに遷移する
            Invoke("Quiz", 2f);
        }
    }

    // voteシーンに遷移する
    void Vote()
    {
        PhotonNetwork.LoadLevel("Vote");
    }

    // Quizシーンに遷移する
    void Quiz()
    {
        PhotonNetwork.LoadLevel("Quiz");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
