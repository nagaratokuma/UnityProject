using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class InstantiatePlayerPanel : MonoBehaviour
{
    // プレハブ格納用
    public GameObject PanelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Roomの人数を取得する
        //int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        int counter = 0;
        // VotePanelにPlayerPanelをRoomの自分以外の人数分追加する
        foreach (var player in PhotonNetwork.PlayerListOthers)
        {
            // プレハブをインスタンス化
            var playerPanel = Instantiate(PanelPrefab);
            // クローンしたオブジェクトの名前を変更する
            var name = "PlayerPanel" + counter;
            playerPanel.name = name;
            // playerPanelの子objectのPlayerNameTextにプレイヤー名を表示する
            playerPanel.transform.Find("PlayerName").GetComponent<Text>().text = player.NickName;

            playerPanel.transform.SetParent(GameObject.Find("VotePanel/upper").transform, false);
            counter++;
        }
        
        //VotePanel を無効化する
        GameObject.Find("VotePanel").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
