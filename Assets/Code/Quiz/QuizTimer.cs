using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizTimer : MonoBehaviour
{
    public float width = 1100;
    public float height = 14;
    //タイマーの時間
    public float LimitTime = 30;
    //経過時間
    private float ElapsedTime = 0;
    public RectTransform rectTransform;

    private bool IsTimeOver = true;

    // Start is called before the first frame update
    void Start()
    {
        float width = 1100;
        float height = 14;       

        // タイマーの時間を設定
        // シーンがQuizの時は
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Quiz")
        {
            LimitTime = 60;
        }
        // シーンがQuiz2の時は
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Quiz2")
        {
            LimitTime = 60;
        }
        // シーンがQuiz3の時は
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Quiz3")
        {
            LimitTime = 300;
        }
        // シーンがVoteの時は
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Vote")
        {
            LimitTime = 60;
        }
        
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    //タイマーを表すバーを減らす
    void TimerBar()
    {
        //新しいサイズ
        float NewWidth;

        //経過時間を加算
        ElapsedTime += Time.deltaTime;
        NewWidth = width - width * (ElapsedTime/ LimitTime);
        //サイズを更新する
        rectTransform.sizeDelta = new Vector2(NewWidth, height);
    }

    // Update is called once per frame
    void Update()
    {
        //  タイムオーバーしてない時
        if (IsTimeOver)
        {
            TimerBar();

            //経過時間が制限時間を超えたら
            if (ElapsedTime >= LimitTime)
            {
                // シーンがQuizの時は
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Quiz")
                {
                    if (Quiz.instance.isSent == false)
                    {
                        Quiz.instance.SendPlayerAnswer(false, "時間切れ");
                        IsTimeOver = false;
                    }
                }

                // シーンがVoteの時は
                else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Vote")
                {
                    
                    IsTimeOver = false;

                }
            }
        }
    }
}
