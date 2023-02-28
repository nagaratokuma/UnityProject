using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class InstaniatePlayerResult : MonoBehaviour
{
    // PlayerResultプレハブ
    public GameObject PlayerResultPrefab;

    // PlayerResultを表示するグループ
    public GameObject ResultGroup;

    

    // Start is called before the first frame update
    void Start()
    {
        // ルームのプレイヤー分PlayerResultを生成する
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int QuizCount = 0;
            int VoteCount = 0;
            int DeceivedCount = 0;
            if (player.CustomProperties["DC"] != null)
            {
                DeceivedCount = (int)player.CustomProperties["DC"];
            }
            if (player.CustomProperties["VC"] != null)
            {
                VoteCount = (int)player.CustomProperties["VC"];
            }
            if (player.CustomProperties["QC"] != null)
            {
                QuizCount = (int)player.CustomProperties["QC"];
            }

            int score = DeceivedCount*10 + VoteCount*5 + QuizCount*5;

            // PlayerResultを生成する
            GameObject playerResult = Instantiate(PlayerResultPrefab) as GameObject;
            // PlayerResultの親をResultGroupにする
            playerResult.transform.SetParent(ResultGroup.transform, false);
            // PlayerResultの名前をIDに設定する
            playerResult.name = player.ActorNumber.ToString();
            // PlayerResultの子要素のPlayerNameTextのTextをプレイヤーのニックネームに設定する
            playerResult.transform.Find("PlayerName").GetComponent<Text>().text = player.NickName;
    
            // PlayerResultの子要素のQuizCountのTextをプレイヤーのスコアに設定する
            playerResult.transform.Find("QuizCount").GetComponent<Text>().text = QuizCount.ToString();
            // PlayerResultの子要素のVoteCountのTextをプレイヤーのスコアに設定する
            playerResult.transform.Find("VoteCount").GetComponent<Text>().text = VoteCount.ToString();
            // PlayerResultの子要素のDeceivedCountのTextをプレイヤーのスコアに設定する
            playerResult.transform.Find("DeceivedCount").GetComponent<Text>().text = DeceivedCount.ToString();
            // PlayerResultの子要素のPointResultのTextをプレイヤーのスコアに設定する
            playerResult.transform.Find("PointResult").GetComponent<Text>().text = score.ToString();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
