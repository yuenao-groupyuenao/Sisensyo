using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{

    [SerializeField] Gamemode selectedGamemode;

    // ボタンが押された場合、今回呼び出される関数
    public void OnClick()
    {
        ScoreManager.isClear = false;
        ScoreManager.gameMode = selectedGamemode;
        Debug.Log("押された!:" + selectedGamemode);  // ログを出力

        switch (selectedGamemode)
        {
            case Gamemode.Easy:
                CountDownScript.inputScriptMinute = 0;
                CountDownScript.inputScriptSecond = 30;
                break;
            case Gamemode.Normal:
                CountDownScript.inputScriptMinute = 5;
                CountDownScript.inputScriptSecond = 00;
                break;
        }
            SceneManager.LoadScene("Sisensyo");
    }
}
