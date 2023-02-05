using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance_ = default;
    public AudioClip mainBgm = default;
    public AudioSource audioSource = default;
    // Start is called before the first frame update
    void Awake()
    {
        instance_ = this;
    }
    void Start()
    {
        audioSource = gameObject.GetComponentMust<AudioSource>();
        audioSource.volume = 0.5f;
        audioSource.clip = mainBgm;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMainBgm()
    {
        audioSource.Play();
    }
    public void StopMainBgm()
    {
        audioSource.Stop();
    }
}
