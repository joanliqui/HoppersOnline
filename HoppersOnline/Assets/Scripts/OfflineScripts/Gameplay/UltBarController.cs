using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltBarController : MonoBehaviour
{
    public float progress;
    [SerializeField] Image progressImage;
    [SerializeField] Color utlimateColor;
    [SerializeField] Image hopperImage;
    BaseHopper hopper;

    //Component References
    AudioSource source;

    private void Start()
    {
        progress = 0;
        progressImage.fillAmount = 0;

        source = GetComponent<AudioSource>();
    }

    public void SetHopper(BaseHopper hop)
    {
        if(hopper != null)
        {
            hopper.OnUltCharging -= UpdateBar;
        }
        hopperImage.sprite = hop.HopperdCard.characterSprite;
        hopper = hop;
        hopper.OnUltCharging += UpdateBar;
    }

    private void OnDisable()
    {
        if (hopper != null) hopper.OnUltCharging -= UpdateBar;
    }

    public void UpdateBar(float p)
    {
        progressImage.color = Color.white;
        progress = Mathf.InverseLerp(0, hopper.Cooldown, p);
        progressImage.fillAmount = progress;

        if(progress >= 1)
        {
            progressImage.color = utlimateColor;
        }
    }
}
