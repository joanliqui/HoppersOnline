using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Biome", menuName = "HoppersMenu/Biome")]
public class Biome : ScriptableObject
{
    public BiomeObject[] zones;
    public GameObject background;
}

[Serializable]
public class Zone
{
    public GameObject zone;
    public Vector3 spawnPoint;

    public void SetSpawnPoint()
    {
        this.spawnPoint = zone.transform.Find("Connector").transform.position;
    }
}
