using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateTest : MonoBehaviour
{
    public GameObject originObject;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(originObject, new Vector3(-1.0f, 0.0f, 0.0f), Quaternion.identity);
    }

}
