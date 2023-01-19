using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] Biome[] biomes;
    [SerializeField] BiomeObject connectorZone;
    [SerializeField] int nZonesToConnector = 4;
    [SerializeField] int maxBiomesIn = 4;


    public bool deleteZone;
    public bool addZone;
    public bool instantiateConectiorZone;


    private Vector2 connectorSpawnPoint;
    private int actualBiome;
    private int nBiomeZonesIn = 0;
    private int lastBiome = -1;
    
    Queue<BiomeObject> zonesInGame;

    private void Start()
    {
        zonesInGame = new Queue<BiomeObject>();
        connectorSpawnPoint = GameObject.FindGameObjectWithTag("InitialConnector").transform.position;

        InitialBuildMap();
    }

    public void AddZone()
    {
        while (zonesInGame.Count >= maxBiomesIn)
        {
            BiomeObject dequeuedZone = zonesInGame.Dequeue();
            Destroy(dequeuedZone.gameObject);
        }

        if(nBiomeZonesIn == nZonesToConnector)
        {
            SwapBiome();
        }

        InstantiateRandomZone();

    }

    public void InitialBuildMap()
    {
        actualBiome = GetRandomBiome();
        for (int i = 0; i < maxBiomesIn; i++)
        {
            InstantiateRandomZone();
        }
    }

    /// <summary>
    /// Cambia el bioma y resetea el contador de Zonas
    /// </summary>
    private void SwapBiome()
    {
        actualBiome = GetRandomBiome();
        BiomeObject zona = Instantiate(connectorZone, connectorSpawnPoint, Quaternion.identity);
        connectorSpawnPoint = zona.GetConnectorPoint().position;

        zonesInGame.Enqueue(zona);
        nBiomeZonesIn = 0;
    }

    private void InstantiateRandomZone()
    {
        int randZone; //Se genera un numero random entre 0 y el maximo de zonas que tiene el bioma que le pasamos
        randZone = Random.Range(0, biomes[actualBiome].zones.Length);

        BiomeObject newZone = Instantiate(biomes[actualBiome].zones[randZone], connectorSpawnPoint, Quaternion.identity);
        connectorSpawnPoint = newZone.GetConnectorPoint().position;
        nBiomeZonesIn++;
        zonesInGame.Enqueue(newZone);
    }

    private int GetRandomBiome()
    {
        int n;
        do
        {
            n = Random.Range(0, biomes.Length);

        } while (n == lastBiome);


        lastBiome = n;
        return n; ;
    }
}
