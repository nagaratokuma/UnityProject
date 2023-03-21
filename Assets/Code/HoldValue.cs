using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldValue : MonoBehaviour
{
    public static int questionNumber = 0;

    public static void SetQuestionNumber(int num)
    {
        questionNumber = num;
    }
}
