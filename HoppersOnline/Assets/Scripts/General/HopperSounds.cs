using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperSounds : MonoBehaviour
{
    AudioSource source;
    [SerializeField] AudioClip stepSounds;
    [SerializeField] AudioClip ultimateSound;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0.3f;
    }

    public void StepSounds()
    {
        source.clip = stepSounds;
        source.volume = 0.3f;
        source.pitch = Random.Range(0, 1.5f);
        source.Play();
    }

    public void UltimateSound()
    {
        source.clip = ultimateSound;
        source.Play();
    }
}
