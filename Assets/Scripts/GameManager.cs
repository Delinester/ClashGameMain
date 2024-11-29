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

[System.Serializable]
public class GameData
{
    public string matchID;

    public int goldTeam1;
    public int foodTeam1;
    public int mineralsTeam1;

    public int goldTeam2;
    public int foodTeam2;
    public int mineralsTeam2;

    public GameData()
    {
        matchID = string.Empty;
        goldTeam1 = 0;
        foodTeam1 = 0;
        mineralsTeam1 = 0;
        goldTeam2 = 0;
        foodTeam2 = 0;
        mineralsTeam2 = 0;
    }
    public GameData(GameData newData)
    {
        Copy(newData);
    }

    public void Copy(GameData newData)
    {
        this.goldTeam1 = newData.goldTeam1;
        this.foodTeam1 = newData.foodTeam1;
        this.mineralsTeam1 = newData.mineralsTeam1;

        this.goldTeam2 = newData.goldTeam2;
        this.foodTeam2 = newData.foodTeam2;
        this.mineralsTeam2 = newData.mineralsTeam2;
    }

    public int GetGold(int teamNum)
    {
        return teamNum == 1 ? goldTeam1 : goldTeam2;
    }
    public int GetFood(int teamNum)
    {
        return teamNum == 1 ? foodTeam1 : foodTeam2;
    }
    public int GetMinerals(int teamNum)
    {
        return teamNum == 1 ? mineralsTeam1 : mineralsTeam2;
    }
    public void AddGold(int gold, int teamNum) 
    {
        if (teamNum == 1) goldTeam1 += gold;
        else goldTeam2 += gold;
    }
    public void AddFood(int food, int teamNum) 
    {
        if (teamNum == 1) foodTeam1 += food;
        else foodTeam2 += food;
    }
    public void AddMinerals(int minerals, int teamNum) 
    {
        if (teamNum == 1) mineralsTeam1 += minerals;
        else mineralsTeam2 += minerals;
    }
}

[System.Serializable]
public class ResourceUpdateMsg
{
    public string matchID;
    public int teamNumber;
    public int amount;
    public Resource resource;

    public ResourceUpdateMsg()
    {
        matchID = string.Empty;
        teamNumber = 0;
        amount = 0;
        resource = Resource.NONE;
    }

    public ResourceUpdateMsg(string matchID, int teamNumber, int amount, Resource resource)
    {
        this.matchID = matchID;
        this.teamNumber = teamNumber;
        this.amount = amount;
        this.resource = resource;
    }
}
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [HideInInspector]
    public BuildingManager buildingManager;

    private Vector3 town1Pos = new Vector3(0, 0, 0);
    private Vector3 town2Pos = new Vector3(0, 100, 0);
    private Vector3 townManagerSpawnPosOffset = new Vector3(5, 5, -1);

    [SerializeField]
    private GameObject townPrefab;
    [SerializeField]
    private GameObject townManagerCharacterPrefab;

    private GameObject townManagerCharacter;
    private GameObject townTeam1;
    private GameObject townTeam2;

    private GameUI gameUI;

    // It will be updated accordingly
    private Dictionary<string, GameData> gameDataHashTable = new Dictionary<string, GameData>();

    private Queue<ResourceUpdateMsg> resourcesUpdateQueue = new Queue<ResourceUpdateMsg>();
    private bool isResourceUpdating = false;


    public GameData GetGameData(string matchID)
    {
        if (!gameDataHashTable.TryGetValue(matchID, out GameData data))
        {
            Debug.LogError("Error in GetGameData");
            return null;
        }
        return data;
    }

    [Server]
    public void InsertGameDataServer(string matchID, GameData gameData)
    {
        if (!gameDataHashTable.TryAdd(matchID, gameData))
        {
            Debug.LogError("Error on adding match data to hashtable");
            return;
        }
        Debug.Log($"Added data to match {matchID}");
    }

    [Command (requiresAuthority = false)]
    public void InsertGameData(string matchID,  GameData gameData)
    {
        InsertGameDataServer(matchID, gameData);
    }

    [Command(requiresAuthority = false)]
    public void CMDUpdateResource(ResourceUpdateMsg msg) 
    {
        resourcesUpdateQueue.Enqueue(msg);
    }

    [Command (requiresAuthority = false)]
    public void CMDUpdateGameData(string matchID, GameData gameData)
    {
        UpdateGameDataAndSync(matchID, gameData);
    }

    [Server]
    private void UpdateGameDataAndSync(string matchID, GameData gameData)
    {
        //GameData gameDataPtr = null;
        if (!gameDataHashTable.TryGetValue(matchID, out GameData gameDataPtr))
        {
            Debug.LogError("Error getting data from hashtable");
            return;
        }

        gameDataPtr.Copy(gameData);

        foreach (PlayerNetworking p in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = p.GetComponent<NetworkIdentity>().connectionToClient;
            RPCUpdateGameDataOnClients(conn, gameData);
        }
    }

    [TargetRpc]
    private void RPCUpdateGameDataOnClients(NetworkConnectionToClient conn, GameData gameData)
    {
        LocalStateManager.instance.localGameData.Copy(gameData);
        gameUI.UpdateBuildingMenuEntries();
    }

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
            gameUI = FindObjectOfType<GameUI>();
            PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();

            if (player.synchronizedPlayerGameData.role == GameRole.TOWN_MANAGER)
            {
                townTeam1 = Instantiate(townPrefab, town1Pos, townPrefab.transform.rotation);
                townTeam2 = Instantiate(townPrefab, town2Pos, townPrefab.transform.rotation);

                Vector3 townManagerSpawnPos = player.synchronizedPlayerGameData.teamNumber == 1 ? town1Pos + townManagerSpawnPosOffset : town2Pos + townManagerSpawnPosOffset;
                //townManagerCharacter = Instantiate(townManagerCharacterPrefab, townManagerSpawnPos, townManagerCharacterPrefab.transform.rotation);
                townManagerCharacter = Instantiate(townManagerCharacterPrefab, player.gameObject.transform);
                townManagerCharacter.transform.position = townManagerSpawnPos;
                LocalStateManager.instance.localPlayerCharacter = townManagerCharacter;
            }
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
        while (resourcesUpdateQueue.Count > 0)
        {
            ResourceUpdateMsg msg = resourcesUpdateQueue.Dequeue();
            string matchID = msg.matchID;
            GameData gameData = GetGameData(matchID);
            if (gameData == null)
            {
                Debug.LogError("NULL GAME DATA");
                return;
            }
            switch (msg.resource)
            {
                case Resource.FOOD: gameData.AddFood(msg.amount, msg.teamNumber); break;
                case Resource.GOLD: gameData.AddGold(msg.amount, msg.teamNumber); break;
                case Resource.MINERAL: gameData.AddMinerals(msg.amount, msg.teamNumber); break;
            }
            UpdateGameDataAndSync(matchID, gameData);
        }
    }
}
