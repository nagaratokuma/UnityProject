using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
using UnityEngine.UI;
 
public class ChangeImage : MonoBehaviour {
 
    public Image image;
    private Sprite sprite;
 
    public void setImage(int num)
    {
        // 画像を変更する
        // 画像を読み込む
        Sprite sprite = Resources.Load<Sprite>("QuizIMG" + num);
        // 画像を変更する
        image.sprite = sprite;
    }

}