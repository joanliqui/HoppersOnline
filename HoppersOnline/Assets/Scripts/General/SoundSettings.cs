using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;

    [SerializeField] AudioMixer masterMixer;
    float cntVolume;

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
        soundSlider.value = PlayerPrefs.GetFloat("fxVolume", 1);
    }
    public void SetMusicVolume(float sliderValue)
    {
        masterMixer.SetFloat("musicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("musicVolume", sliderValue);
    }

    public void SetEffectsVolume(float sliderValue)
    {
        masterMixer.SetFloat("fxVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("fxVolume", sliderValue);

    }
}
