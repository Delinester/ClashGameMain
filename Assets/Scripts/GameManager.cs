using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameRole
{
    TOWN_MANAGER,
    WARRIOR,
    MINER
}

public enum Resource
{
    NONE,
    GOLD,
    FOOD,
    MINERAL
}
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    private Vector3 town1Pos = new Vector3(0, 0, 0);
    private Vector3 town2Pos = new Vector3(0, 100, 0);


    [SerializeField]
    private GameObject townPrefab;
    [SerializeField]
    private GameObject townTeam1;
    [SerializeField]
    private GameObject townTeam2;

    public Vector2 GetTownTeamPosition(int teamNum)
    {
        if (teamNum == 1)
            return townTeam1.gameObject.transform.position;
        else return townTeam2.gameObject.transform.position;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //else if (instance != this)
        //{
        //    Destroy(gameObject);
        //}
    }

    [Command]
    public void PlaceBuilding(BuildingData data, Vector3 position)
    {
        Debug.LogError("Place building is called!!!");
        GameObject obj = Instantiate(data.buildingPrefab, position, data.buildingPrefab.transform.rotation);
        NetworkServer.Spawn(obj);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            townTeam1 = Instantiate(townPrefab, town1Pos, townPrefab.transform.rotation);
            townTeam2 = Instantiate(townPrefab, town2Pos, townPrefab.transform.rotation);
        }
    }
    // Start is called before the first frame update
    
    void Start()
    {
        if (isClient)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
