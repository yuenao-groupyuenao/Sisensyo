using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    private Text resultText;
    private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        resultText = this.gameObject.transform.Find("Gameclear").GetComponent<Text>();
        scoreText = this.gameObject.transform.Find("Score").GetComponent<Text>();


        if (ScoreManager.isClear)
        {
            resultText.text = "GameClear";
            scoreText.text = "Score:" + (CalcScore() * 1000);
        }
        else
        {
            resultText.text = "Failed";
            scoreText.text = "";
        }
        

    }

    int CalcScore()
    {
        int score = CountDownScript.minute * 60;
        score += CountDownScript.seconds;
        return score;
    }

}
