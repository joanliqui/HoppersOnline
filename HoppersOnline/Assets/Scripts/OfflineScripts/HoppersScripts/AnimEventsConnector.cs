using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsConnector : MonoBehaviour
{
    BaseHopper hopper;

    private void Start()
    {
        hopper = GetComponentInParent<BaseHopper>();
    }

    public void EndUltimate()
    {
        hopper.EndUltimate();
    }
}
