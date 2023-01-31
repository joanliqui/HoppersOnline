using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class NetMapBuilder : MonoBehaviour
{
    private static NetMapBuilder _instance;

    [SerializeField] Biome[] biomes;
    [SerializeField] NetBiomeObject connectorZone;
    [SerializeField] int nZonesToConnector = 4;
    [SerializeField] int maxZonesIn = 4;

    //Bacground Related Variables
    private GameObject actualBgGameObject;
    private NetBackgroundController actualBG;
    private int orderInLayer = 0;

    private Vector2 connectorSpawnPoint;
    private Transform cam;
    private int actualBiome;
    private int nBiomeZonesIn = 0;
    private int lastBiome = -1;

    Queue<NetBiomeObject> zonesInGame;

    public UnityEvent<TypeZone> OnCamEnterCollider;
    PhotonView view;
    public static NetMapBuilder Instance { get => _instance; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cam = Camera.main.transform;
        zonesInGame = new Queue<NetBiomeObject>();
        connectorSpawnPoint = GameObject.FindGameObjectWithTag("InitialConnector").transform.position;
        view = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            InitialBuildMap();
            SpawnNewBackground();
        }
        OnCamEnterCollider.AddListener(AddZone);
    }

    public void InitialBuildMap()
    {
        for (int i = 0; i < 3; i++)
        {
            AddZone(TypeZone.None);
        }
    }
    public void OnCamEnterEventCall(TypeZone type) //Se llamará solo en el master client
    {
        if(PhotonNetwork.IsMasterClient)
            OnCamEnterCollider?.Invoke(type);
    }

    public void AddZone(TypeZone type)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (zonesInGame.Count >= maxZonesIn)
            {
                NetBiomeObject dequeuedZone = zonesInGame.Dequeue();
                PhotonNetwork.Destroy(dequeuedZone.gameObject);
            }

            if (nBiomeZonesIn == nZonesToConnector)
            {
                SwapBiome();
            }
            else
            {
                InstantiateRandomZone();
            }

            if (type == TypeZone.Neutral)
            {
                ChangeBackground();
            }
        }
    }

    /// <summary>
    /// Cambia el bioma y resetea el contador de Zonas
    /// </summary>
    private void SwapBiome()
    {
        view.RPC("SetRandomBiomeRPC", RpcTarget.All);
        GameObject zonaObject = PhotonNetwork.Instantiate("BiomeZones/" + connectorZone.name, connectorSpawnPoint, Quaternion.identity);

        NetBiomeObject zona = zonaObject.GetComponent<NetBiomeObject>();
        connectorSpawnPoint = zona.GetConnectorPoint().position;

        zonesInGame.Enqueue(zona);
        nBiomeZonesIn = 0;

        Debug.Log("SwapBiome");
    }
    private void ChangeBackground()
    {
        Debug.Log("ChangeBackground");
        actualBG.transform.parent = null;
        actualBG.DetachBackgroundRPC();
        SpawnNewBackground();

    }

    private void InstantiateRandomZone()
    {
        int randZone; //Se genera un numero random entre 0 y el maximo de zonas que tiene el bioma que le pasamos
        randZone = Random.Range(0, biomes[actualBiome].zones.Length);

        GameObject newZoneObject = PhotonNetwork.Instantiate("BiomeZones/" + biomes[actualBiome].netZones[randZone].name, connectorSpawnPoint, Quaternion.identity);

        NetBiomeObject newZone = newZoneObject.GetComponent<NetBiomeObject>();
        connectorSpawnPoint = newZone.GetConnectorPoint().position;
        nBiomeZonesIn++;
        zonesInGame.Enqueue(newZone);
    }

    private void SpawnNewBackground()
    {
        actualBgGameObject = PhotonNetwork.Instantiate("BiomeZones/"+biomes[actualBiome].netBackground.name, cam.position, Quaternion.identity);
        
        actualBG = actualBgGameObject.GetComponent<NetBackgroundController>();
        actualBG.InitiateBackground(cam, orderInLayer);
        orderInLayer--;
    }


    [PunRPC]
    private void SetRandomBiomeRPC()
    {
        actualBiome =  GetRandomBiome();
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
