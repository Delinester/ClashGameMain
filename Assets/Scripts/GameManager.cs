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

    [HideInInspector]
    public BuildingManager buildingManager;

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

        buildingManager = gameObject.GetComponent<BuildingManager>();
        if (buildingManager == null) Debug.Log("Building manager is null!!");
        //else if (instance != this)
        //{
        //    Destroy(gameObject);
        //}
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
