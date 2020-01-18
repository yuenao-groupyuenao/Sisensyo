using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static Gamemode gameMode;
    public static bool isClear = false;
    public static int score;

}

public enum Gamemode
{
    Easy,
    Normal
}
