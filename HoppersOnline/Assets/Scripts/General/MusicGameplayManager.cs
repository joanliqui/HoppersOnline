using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGameplayManager : MonoBehaviour
{
    [SerializeField] AudioClip[] clips = new AudioClip[2];
    AudioSource source;
    bool oneTake = false;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[0];
        source.Play();
    }

    private void Start()
    {
        NetGameManager.Instance.onGameEnded.AddListener(source.Stop);
    }

    private void Update()
    {
        if (!oneTake)
        {
            if (!source.isPlaying && source.clip == clips[0])
            {
                source.clip = clips[1];
                oneTake = true;
                source.Play();
                source.loop = true;
            }
        }
    }

}
