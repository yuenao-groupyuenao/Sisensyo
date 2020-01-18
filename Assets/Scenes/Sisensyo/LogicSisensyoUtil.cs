using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LogicSisensyo
{
    //便利関数群
    //あとでUtilクラスにするかも

    /// <summary>
    /// intリストからランダムに一つ取り出す。
    /// 取り出された値はリストから削除される。
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    int PickupFromList(List<int> list)
    {
        int index = (int)(list.Count * random.NextDouble());
        int result = list[index];
        list.RemoveAt(index);
        return result;
    }

    /// <summary>
    /// startからstopまでstepの値分加算したintリストを作成する
    /// 例：Range(0,10,2)
    /// [0,2,4,6,8,10]
    /// </summary>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    List<int> Range(int start, int stop, int step)
    {
        //親切な前処理
        //stopが0ならstartを0にしてstartの値までrangeする。
        if (stop == 0)
        {
            stop = start;
            start = 0;
        }

        //返却するListのLengthをstop,start,stepから算出。
        //それが0以下なら0を返す
        int length = System.Math.Max(0, (int)System.Math.Ceiling((stop - start) / (double)step));

        List<int> result = new List<int>();

        for (int i = 0; i < length; i++)
        {
            result.Add(start + step * i);
        }

        return result;
    }

    int IndexToX(int index)
    {
        return index % fieldWidth;
    }

    int IndexToY(int index)
    {
        return index / fieldWidth;
    }

    int XYtoIndex(int x, int y)
    {
        return x + y * fieldWidth;
    }

    int YXtoIndex(int y, int x)
    {
        return XYtoIndex(x, y);
    }
}


