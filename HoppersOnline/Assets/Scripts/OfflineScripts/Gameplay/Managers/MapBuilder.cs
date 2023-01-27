using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapBuilder : MonoBehaviour
{
    private static MapBuilder _instance;

    [SerializeField] Biome[] biomes;
    [SerializeField] BiomeObject connectorZone;
    [SerializeField] int nZonesToConnector = 4;
    [SerializeField] int maxZonesIn = 4;

    //Bacground Related Variables
    private BackgroundController actualBG;
    private int orderInLayer = 0;

    private Vector2 connectorSpawnPoint;
    private Transform cam;
    private int actualBiome;
    private int nBiomeZonesIn = 0;
    private int lastBiome = -1;
    
    Queue<BiomeObject> zonesInGame;

    public UnityEvent<TypeZone> OnCamEnterCollider;

    public static MapBuilder Instance { get => _instance;}

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        cam = Camera.main.transform;
        zonesInGame = new Queue<BiomeObject>();
        connectorSpawnPoint = GameObject.FindGameObjectWithTag("InitialConnector").transform.position;

        InitialBuildMap();
        SpawnNewBackground();
        OnCamEnterCollider.AddListener(AddZone);

    }
    public void InitialBuildMap()
    {
        actualBiome = GetRandomBiome();
        for (int i = 0; i < 3; i++)
        {
            AddZone(TypeZone.None);
        }
    }

    public void OnCamEnterEventCall(TypeZone type)
    {
        OnCamEnterCollider?.Invoke(type);
    } 

    public void AddZone(TypeZone type)
    {
        if (zonesInGame.Count >= maxZonesIn)
        {
            BiomeObject dequeuedZone = zonesInGame.Dequeue();
            Destroy(dequeuedZone.gameObject);
        }

        if(nBiomeZonesIn == nZonesToConnector)
        {
            SwapBiome();
        }
        else
        {
            InstantiateRandomZone();
        }

        if(type == TypeZone.Neutral)
        {
            ChangeBackground();
        }
    }

    /// <summary>
    /// Spawn a new background and set the position and order in layer
    /// </summary>
    private void SpawnNewBackground()
    {
        actualBG = Instantiate(biomes[actualBiome].background, cam.position, Quaternion.identity);
        Debug.Log(actualBG.gameObject.name);
        actualBG.InitiateBackground(cam, orderInLayer);
        orderInLayer--;
    }

    private void ChangeBackground()
    {
        actualBG.DetachBackground();
        SpawnNewBackground();

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
