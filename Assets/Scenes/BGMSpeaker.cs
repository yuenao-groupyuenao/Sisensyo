using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSpeaker : MonoBehaviour
{
    private static bool isPlay = false;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        audioSource = this.GetComponent<AudioSource>();
        audioSource.loop = true;
        if (!isPlay)
        {
            audioSource.Play();
            isPlay = true;
        }
    }
}
