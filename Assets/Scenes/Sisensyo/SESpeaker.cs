using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SESpeaker : MonoBehaviour
{

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        audioSource = this.GetComponent<AudioSource>();
    }

    public void playSE()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
