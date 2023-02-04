using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGameplayManager : MonoBehaviour
{
    [SerializeField] AudioClip[] clips = new AudioClip[2];
    AudioSource source;
    bool oneTake = false;

    bool canSound = true;
    [SerializeField] AudioClip endSound;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[0];
        source.Play();
    }

    private void Start()
    {
        NetGameManager.Instance.onGameEnded.AddListener(EndMusic);
    }

    private void Update()
    {
        if (!canSound) return;

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

    private void EndMusic()
    {
        canSound = false;
        source.Stop();
        source.clip = endSound;
        source.Play();
    }

}
