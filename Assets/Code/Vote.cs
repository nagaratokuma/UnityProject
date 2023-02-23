using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vote : MonoBehaviour
{
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
    }

    public void SendPlayerVote(string Votedname)
    {
        Debug.Log("Vote: " + Votedname);
    }
    
    // Voteボタンが押ボタンが押されたときに呼び出される
    public void OnClickVoteButtonMethod()
    {
        // 押されたボタンの名前を取得する
        var buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(buttonName);
        /*
        // 押されたボタンの名前から投票先のプレイヤー番号を取得する
        var vote = buttonName;
        // 投票先のプレイヤー番号を送信する
        SendPlayerVote(vote);
        */
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
