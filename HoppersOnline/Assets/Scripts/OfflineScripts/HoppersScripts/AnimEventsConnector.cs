using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsConnector : MonoBehaviour
{
    Transform parent;

    NetBaseHopper netHopper;
    BaseHopper hopperOffline;

    private void Start()
    {
        transform.parent.TryGetComponent<NetBaseHopper>( out netHopper);
        if(netHopper == null)
        {
            transform.parent.TryGetComponent<BaseHopper>(out hopperOffline);
        }
    }

    public void EndUltimate()
    {
        if (hopperOffline)
        {
            hopperOffline.EndUltimate();
        }
        else
        {
            netHopper.EndUltimate();
        }
    }
}
