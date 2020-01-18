using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{

    [SerializeField]
    private int minute;
    [SerializeField]
    private int seconds;
    

    private float rawTime;
    //　前のUpdateの時の秒数
    private float oldrawTime;
    //　タイマー表示用テキスト
    private Text timerText;

    void Start()
    {
        minute = 0;
        seconds = 0;
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
            seconds++;
            rawTime = rawTime - 1f;
        }

        if (seconds >= 60f)
        {
            minute++;
            seconds = seconds - 60;
        }
        //　値が変わった時だけテキストUIを更新
        if ((int)(rawTime * 100) != (int)oldrawTime)
        {
            timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + "." + ((int)(rawTime * 100)).ToString("00");
        }
        oldrawTime = rawTime;
    }
}