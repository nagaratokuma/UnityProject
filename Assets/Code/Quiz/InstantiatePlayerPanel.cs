using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class InstantiatePlayerPanel : MonoBehaviour
{
    // プレハブ格納用
    public GameObject PanelPrefab;

    // Start is called before the first frame update

    void Start()
    {
        int counter = 0;
        // VotePanelにPlayerPanelをRoomの人数分追加する
        foreach (var player in PhotonNetwork.PlayerList)
        {
            // プレハブをインスタンス化
            var playerPanel = Instantiate(PanelPrefab);
            // クローンしたオブジェクトの名前を変更する
            var name = player.ActorNumber.ToString();
            playerPanel.name = name;
            // クローンしたオブジェクトの子オブジェクトVoteButtonのOnClikにVoteクラスのSendPlayerVoteを設定する
            playerPanel.transform.Find("VoteButton").GetComponent<Button>().onClick.AddListener(() => Vote.instance.OnClickVoteButtonMethod());
            // クローンしたオブジェクトの子オブジェクトVoteButtonの名前を変更する
            //playerPanel.transform.Find("VoteButton").name = name;
            
            // playerPanelの子objectのPlayerNameTextにプレイヤー名を表示する
            playerPanel.transform.Find("PlayerName").GetComponent<Text>().text = player.NickName;
            
            // シーンがVoteの時は
            if (SceneManager.GetActiveScene().name == "Vote")
            {
                Debug.Log("Vote");
                // playerPanelの子objectのSpeechBaloonを無効にする
                playerPanel.transform.Find("SpeechBaloon").gameObject.SetActive(false);
                playerPanel.transform.Find("VoteButton").gameObject.SetActive(true);

                // 自分のplayerPanelの子objectのVoteButtonを押せないようにする
                if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //playerPanel.transform.Find("VoteButton").GetComponent<Button>().interactable = false;
                }
                
            }
            
            playerPanel.transform.SetParent(GameObject.Find("VotePanel/upper").transform, false);
            counter++;
        }
        
        //VotePanel を無効化する
        //GameObject.Find("VotePanel").SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
