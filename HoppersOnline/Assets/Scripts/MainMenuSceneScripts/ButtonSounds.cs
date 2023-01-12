using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof (AudioSource))]
public class ButtonSounds : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] AudioClip[] clipsSoundBtn;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void OnSelect(BaseEventData eventData)
    {
        SoundOnButtonEnter();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundOnButtonEnter();
    }

    private void SoundOnButtonEnter()
    {
        source.clip = clipsSoundBtn[0];
        source.Play();
    }

    public void SoundOnButtonSubmit()
    {
        source.clip = clipsSoundBtn[1];
        source.Play();
    }
}
