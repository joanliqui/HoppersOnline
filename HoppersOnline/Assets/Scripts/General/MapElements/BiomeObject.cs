using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeObject : MonoBehaviour
{
    [SerializeField] TypeZone typeZone;
    [SerializeField] Transform connectorPoint;

    [Header("Enemies Space")]
    [SerializeField] Transform[] enemySpawnPoints;

    private Collider2D col;
    float ps;

    bool addZone = false;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public Transform GetConnectorPoint()
    {
        if (connectorPoint != null)
            return connectorPoint;
        else
            Debug.LogError(string.Format("Connector Point of {0} is not set", gameObject.name));
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            col.enabled = false;
            MapBuilder.Instance.OnCamEnterEventCall(typeZone);
        }
    }
}

public enum TypeZone
{
    None, Cloud, Jungle, Neutral
};
