using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof (AudioSource))]
public class ButtonSounds : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] AudioClip[] clipsSoundBtn;
    private AudioSource source;
    private Button btn;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        btn = GetComponent<Button>();
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (!btn.interactable) return;

        SoundOnButtonEnter();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!btn.interactable) return;
        SoundOnButtonEnter();
    }

    private void SoundOnButtonEnter()
    {
        if (!btn.interactable) return;
        source.clip = clipsSoundBtn[0];
        source.Play();
    }

    public void SoundOnButtonSubmit()
    {
        if (!btn.interactable) return;
        source.clip = clipsSoundBtn[1];
        source.Play();
    }
}
