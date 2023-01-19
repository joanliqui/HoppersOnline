using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    Parallax parallax;
    Parallax[] parallaxes = new Parallax[2];
    SpriteRenderer sr;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            parallaxes[i] = transform.GetChild(i).GetComponent<Parallax>();
        }

        parallax = GetComponent<Parallax>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void InitiateBackground(Transform parent, int orderInLayer)
    {
        gameObject.transform.SetParent(parent);
        gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 15f);
        sr.sortingOrder = orderInLayer;
    }

    /// <summary>
    /// Set parent to null and stop the parallaxes
    /// </summary>
    public void DetachBackground()
    {
        gameObject.transform.parent = null;
        parallax.ParallaxState(false);
        foreach (Parallax item in parallaxes)
        {
            item.ParallaxState(false);
        }
    }
}
