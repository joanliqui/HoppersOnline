using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeObject : MonoBehaviour
{
    private enum TypeZone
    {
        Cloud, Jungle, Neutral
    };

    [SerializeField] TypeZone typeZone;
    [SerializeField] Transform connectorPoint;

    [Header("Enemies Space")]
    [SerializeField] Transform[] enemySpawnPoints;

    private BoxCollider2D col;
    float ps;

    bool addZone = false;


    public Transform GetConnectorPoint()
    {
        if (connectorPoint != null)
            return connectorPoint;
        else
            Debug.LogError(string.Format("Connector Point of {0} is not set", gameObject.name));
        return null;
    }
}
