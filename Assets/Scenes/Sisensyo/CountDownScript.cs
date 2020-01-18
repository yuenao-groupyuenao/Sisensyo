using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class CountDownScript : MonoBehaviour
{
    [SerializeField]
    private int inputUnityMinute;
    [SerializeField]
    private int inputUnitySeconds;
    [SerializeField]
    private UnityEvent Timeup = new UnityEvent();

    public static int minute = 0;
    public static int seconds = 0;

    public static int inputScriptMinute;
    public static int inputScriptSecond;

    private float rawTime;
    //　前のUpdateの時の秒数
    private float oldrawTime;
    //　タイマー表示用テキスト
    private Text timerText;

    void Start()
    {
        if(!(inputUnityMinute == 0) && !(inputUnitySeconds == 0))
        {
            minute = inputUnityMinute;
            seconds = inputUnitySeconds;
        }
        else
        {
            minute = inputScriptMinute;
            seconds = inputScriptSecond;
        }

        //小数点秒が99からはじまるので調整
        if(0 < seconds)
        {
            seconds--;
        }
        else if(seconds == 0 && 0 < minute)
        {
            seconds = 59;
            minute--;
        }

        rawTime = 0f;
        oldrawTime = 0f;
        timerText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        //Time.deltaTimeはリアル時間とずれる可能性あり
        rawTime += Time.deltaTime;
        if (rawTime >= 1f)
        {
            seconds--;
            rawTime = rawTime - 1f;
        }

        if (seconds < 0)
        {
            if(minute == 0)
            {
                Timeup.Invoke();
                return;
            }
            minute--;
            seconds = 59;
        }
        //　値が変わった時だけテキストUIを更新
        if ((int)(rawTime * 100) != (int)oldrawTime)
        {
            timerText.text = minute.ToString("00") + ":" 
                + ((int)seconds).ToString("00") + "." 
                + ((100 - (int)(rawTime * 100))).ToString("00");
        }
        oldrawTime = rawTime;
    }
}