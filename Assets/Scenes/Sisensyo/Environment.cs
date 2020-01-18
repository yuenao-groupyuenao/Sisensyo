using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        ViewEnvironment.paiMarginWidth = 1;
        ViewEnvironment.paiMarginHeight = 1;
        ViewEnvironment.paiFirstPOSX = 1.5;
        ViewEnvironment.paiFirstPOSY = -1.5;
        ViewEnvironment.paiPOSZ = 10;
    }

}

public static class ViewEnvironment
{
    public static double paiMarginWidth { get; set; }
    public static double paiMarginHeight { get; set; }
    public static double paiFirstPOSX { get; set; }
    public static double paiFirstPOSY { get; set; }
    public static double paiPOSZ { get; set; }
}
