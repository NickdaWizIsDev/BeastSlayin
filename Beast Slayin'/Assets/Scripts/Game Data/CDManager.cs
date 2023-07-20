using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDManager : MonoBehaviour
{
    public FloatVariable railgunCD;
    public AudioClip railgunReady;

    private AudioSource audioSource;

    public bool isChargingRailcannon = false;
    private bool isPlaying;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (railgunCD.value < 16f)
        {
            railgunCD.value += Time.deltaTime;
            isChargingRailcannon = true;
            isPlaying = false;
        }

        else if (railgunCD.value >= 16f && !isPlaying)
        {
            isPlaying = true;
            audioSource.PlayOneShot(railgunReady, 0.2f);
            isChargingRailcannon = false;
        }
    }
}
