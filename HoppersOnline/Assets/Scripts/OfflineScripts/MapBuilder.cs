using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] Biome[] biomes;
    [SerializeField] GameObject connectorZone;
    [SerializeField] int nZonesToConnector = 4;
    [SerializeField] int maxBiomesIn = 4;


    public bool deleteZone;
    public bool addZone;
    public bool instantiateConectiorZone;


    private Vector2 connectorSpawnPoint;
    private int actualBiome;
    private int nBiomesIn = 0;
    private int lastBiome = -1;
    
    Queue<BiomeObject> zonesInGame;

    private void Start()
    {
        zonesInGame = new Queue<BiomeObject>();
        connectorSpawnPoint = GameObject.FindGameObjectWithTag("InitialConnector").transform.position;

        InitialBuildMap();
    }
    public void BuildMap()
    {
        
    }

    public void InitialBuildMap()
    {
        actualBiome = GetRandomBiome();
        for (int i = 0; i < nZonesToConnector; i++)
        {
            InstantiateRandomeZone();
        }
    }

    private void InstantiateRandomeZone()
    {
        int randZone; //Se genera un numero random entre 0 y el maximo de zonas que tiene el bioma que le pasamos
        randZone = Random.Range(0, biomes[actualBiome].zones.Length);

        BiomeObject newZone = Instantiate(biomes[actualBiome].zones[randZone], connectorSpawnPoint, Quaternion.identity);
        connectorSpawnPoint = newZone.GetConnectorPoint().position;
        nBiomesIn++;
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
