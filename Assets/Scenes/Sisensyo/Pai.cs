using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pai : MonoBehaviour
{
    public int paiTypeId { get; set; }
    public int index { get; set; }

    public Pai(int paiTypeId, int index)
    {
        this.paiTypeId = paiTypeId;
        this.index = index;
    }
   
}
