using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HoldValue : MonoBehaviour
{
    public static int questionNumber = 0;

    public static string SelectedFile = "";

    // 問題番号を格納する変数
    public static int QuizNumInt = 1;

    // 問題の最大数を格納する変数
    public static int MaxQuizNum;

    private Hashtable RoomHashtable = new ExitGames.Client.Photon.Hashtable();

    public static void SetQuestionNumber(int num)
    {
        questionNumber = num;
    }

    public static void SetMaxQuizNum(int num)
    {
        MaxQuizNum = num;
    }
}
