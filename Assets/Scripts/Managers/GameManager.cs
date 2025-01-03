using Mirror;
using NavMeshPlus.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameRole
{
    TOWN_MANAGER,
    WARRIOR,
    MINER,
    ADMIN
}

public enum Resource
{
    NONE,
    GOLD,
    FOOD,
    GOLD_ORE,
    MINERAL,
    BARBARIAN,
    MERCENARY,
    HEAVY_KNIGHT
}

public enum PuppetType
{
    TOWN_MANAGER,
    WARRIOR,
    MINER,
    BARBARIAN,
    MERCENARY,
    HEAVY_KNIGHT,
    GOBLIN,
    ARMY
}

[System.Serializable]
public class GameData
{
    public string matchID;

    public int goldTeam1;
    public int foodTeam1;
    public int mineralsTeam1;
    public int minerGoldOre1;
    public int barbarians1;
    public int mercenaries1;
    public int heavyKnights1;

    public int goldTeam2;
    public int foodTeam2;
    public int mineralsTeam2;
    public int minerGoldOre2;
    public int barbarians2;
    public int mercenaries2;
    public int heavyKnights2;

    public GameData()
    {
        matchID = string.Empty;
        goldTeam1 = 0;
        foodTeam1 = 0;
        mineralsTeam1 = 0;
        minerGoldOre1 = 0;
        barbarians1 = 0;
        mercenaries1 = 0;   
        heavyKnights1 = 0;

        goldTeam2 = 0;
        foodTeam2 = 0;
        mineralsTeam2 = 0;
        minerGoldOre2 = 0;
        barbarians2 = 0;
        mercenaries2 = 0;
        heavyKnights2 = 0;
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
        this.minerGoldOre1 = newData.minerGoldOre1;
        this.barbarians1 = newData.barbarians1;
        this.mercenaries1 = newData.mercenaries1;
        this.heavyKnights1 = newData.heavyKnights1;

        this.goldTeam2 = newData.goldTeam2;
        this.foodTeam2 = newData.foodTeam2;
        this.mineralsTeam2 = newData.mineralsTeam2;
        this.minerGoldOre2 = newData.minerGoldOre2;
        this.barbarians2 = newData.barbarians2;
        this.mercenaries2 = newData.mercenaries2;
        this.heavyKnights2 = newData.heavyKnights2;
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
    public int GetGoldOre(int teamNum)
    {
        return teamNum == 1 ? minerGoldOre1 : minerGoldOre2;
    }
    public int GetBarbarians(int teamNum)
    {
        return teamNum == 1 ? barbarians1 : barbarians2;
    }
    public int GetMercenaries(int teamNum)
    {
        return teamNum == 1 ? mercenaries1 : mercenaries2;
    }
    public int GetHeavyKnights(int teamNum)
    {
        return teamNum == 1 ? heavyKnights1 : heavyKnights2;
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
    public void AddGoldOre(int ore, int teamNum)
    {
        if (teamNum == 1) minerGoldOre1 += ore;
        else minerGoldOre2 += ore;
    }
    public void AddBarbarians(int amount, int teamNum)
    {
        if (teamNum == 1) barbarians1 += amount;
        else barbarians2 += amount;
    }
    public void AddMercenaries(int amount, int teamNum)
    {
        if (teamNum == 1) mercenaries1 += amount;
        else mercenaries2 += amount;
    }
    public void AddHeavyKnights(int amount, int teamNum)
    {
        if (teamNum == 1) heavyKnights1 += amount;
        else heavyKnights2 += amount;
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
    [HideInInspector]
    public MineManager mineManager;
    [HideInInspector]
    public BattlesManager battlesManager;

    [SerializeField]
    private GameObject navMesh;

    private Vector3 town1Pos = new Vector3(0, 0, 0);
    private Vector3 town2Pos = new Vector3(0, 100, 0);
    private Vector3 townManagerSpawnPosOffset = new Vector3(5, 5, -1);

    private Vector3 minerShack1Location = new Vector3(0, 500, 0);
    private Vector3 minerShack2Location = new Vector3(0, 1000, 0);
    private Vector3 minerSpawnPosOffset = new Vector3(1, -3, -1);

    private Vector3 worldMapSpawnPos = new Vector3(0, 3000, 0);
    private Vector3 worldMapCharacterSpawnOffset = new Vector3(0, 5f, 0);

    List<CharacterControllerBase> puppetsList = new List<CharacterControllerBase>();

    [Header("Avatars and Profile")]
    [SerializeField]
    public List<Sprite> avatarsList;
    [SerializeField]
    public List<Sprite> achievementsList;

    [Header("Town stuff")]
    [SerializeField]
    private GameObject townPrefab;
    [SerializeField]
    private GameObject townManagerCharacterPrefab;

    private GameObject townManagerCharacter;
    private GameObject townTeam1;
    private GameObject townTeam2;

    [Header("Mining stuff")]
    [SerializeField]
    private GameObject minerCharacterPrefab;
    [SerializeField]
    private GameObject minerShackPrefab;

    private GameObject minerCharacter;
    private GameObject minerShack1;
    private GameObject minerShack2;

    [Header("Warrior stuff")]
    [SerializeField]
    private GameObject worldMap;
    [SerializeField]
    private GameObject worldMapCharacterPrefab;
    // VERY BAD IDEA- NOT SCALABLE - KOSTIL
    [SerializeField]
    private GameObject barbarianPrefab;
    [SerializeField]
    private GameObject mercenaryPrefab;
    [SerializeField]
    private GameObject heavyKnightPrefab;
    [SerializeField]
    private GameObject worldMapArmyPrefab;
    [SerializeField]
    private TroopData[] troopDataArray;
    ///
    private GameObject worldMapCharacter;

    private GameUI gameUI;

    // It will be updated accordingly
    private Dictionary<string, GameData> gameDataHashTable = new Dictionary<string, GameData>();

    private Queue<ResourceUpdateMsg> resourcesUpdateQueue = new Queue<ResourceUpdateMsg>();
    private bool isResourceUpdating = false;


    public GameData GetGameData(string matchID)
    {
        if (!gameDataHashTable.TryGetValue(matchID, out GameData data))
        {
            Debug.LogError("Error in GetGameData of matchID: " + matchID);
            return null;
        }
        return data;
    }

    public Vector3 GetMineShackLocation(int team)
    {
        return team == 1 ? minerShack1Location : minerShack2Location;
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

        if (LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.role == GameRole.TOWN_MANAGER)
            gameUI.UpdateBuildingMenuEntries();
    }

   
    public void BakeNavMesh()
    {
        navMesh.GetComponent<NavMeshSurface>().BuildNavMeshAsync();
        foreach (TroopAI troop in FindObjectsOfType<TroopAI>())
        {
            troop.ResetPath();
        }
    }
    public Vector2 GetTownTeamPosition(int teamNum)
    {
        if (teamNum == 1)
            return townTeam1.gameObject.transform.position;
        else return townTeam2.gameObject.transform.position;
    }

    public GameObject GetTroopPrefab(Resource type)
    {
        GameObject character = null;
        switch (type)
        {
            case Resource.BARBARIAN: character = barbarianPrefab; break;
            case Resource.MERCENARY: character = mercenaryPrefab; break;
            case Resource.HEAVY_KNIGHT: character = heavyKnightPrefab; break;
        }
        return character;
    }

    public ArmySpawnBounds GetTownArmySpawnBounds(int teamNum)
    {
        GameObject townObject = null;
        if (teamNum == 1) townObject = townTeam1;
        else townObject = townTeam2;

        return townObject.GetComponentInChildren<ArmySpawnBounds>();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        buildingManager = gameObject.GetComponent<BuildingManager>();
        if (buildingManager == null) Debug.LogError("Building manager is null!!");
        mineManager = GetComponent<MineManager>();
        if (mineManager == null) Debug.LogError("Mine manager is NULL");
        battlesManager = GetComponent<BattlesManager>();
        if (battlesManager == null) Debug.LogError("Battles manager is null");
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


            GameRole role = player.synchronizedPlayerGameData.role;


            townTeam1 = Instantiate(townPrefab, town1Pos, townPrefab.transform.rotation);
            townTeam2 = Instantiate(townPrefab, town2Pos, townPrefab.transform.rotation);


            minerShack1 = Instantiate(minerShackPrefab, minerShack1Location, minerCharacterPrefab.transform.rotation);
            minerShack2 = Instantiate(minerShackPrefab, minerShack2Location, minerCharacterPrefab.transform.rotation);


            GameObject worldMapInstance = Instantiate(worldMap, worldMapSpawnPos, worldMap.transform.rotation);

            BakeNavMesh();

            //mineManager.CMDGenerateMine(player.synchronizedPlayerGameData.matchPtr.matchID);

            if (role == GameRole.TOWN_MANAGER)
            {
                Debug.Log("You are TOWN MANAGER");
                gameUI.TurnTownManagerUI(true);
                gameUI.TurnMinerUI(false);
                gameUI.TurnWarriorUI(false);
                gameUI.TurnAdminUI(false);
                Vector3 townManagerSpawnPos = player.synchronizedPlayerGameData.teamNumber == 1 ? town1Pos + townManagerSpawnPosOffset : town2Pos + townManagerSpawnPosOffset;
                //townManagerCharacter = Instantiate(townManagerCharacterPrefab, townManagerSpawnPos, townManagerCharacterPrefab.transform.rotation);

                // The character is invisible for all clients
                townManagerCharacter = Instantiate(townManagerCharacterPrefab, player.gameObject.transform);
                townManagerCharacter.transform.position = townManagerSpawnPos;
                LocalStateManager.instance.localPlayerCharacter = townManagerCharacter;
                string hash = LobbyManager.instance.GenerateRandomString(20);
                townManagerCharacter.GetComponent<CharacterControllerBase>().SetHash(hash);
                CMDSpawnPuppetOnClients(player.synchronizedPlayerGameData.matchPtr.matchID, player.GetUserData().username, hash,PuppetType.TOWN_MANAGER, townManagerSpawnPos);
            }

            else if (role == GameRole.MINER)
            {
                Debug.Log("You are MINER");
                gameUI.TurnMinerUI(true);
                gameUI.TurnTownManagerUI(false);
                gameUI.TurnWarriorUI(false);
                gameUI.TurnAdminUI(false);
                Vector3 minerSpawnPos = player.synchronizedPlayerGameData.teamNumber == 1 ? minerShack1Location + minerSpawnPosOffset : minerShack2Location + minerSpawnPosOffset;


                // Miner should be visible for all clients!
                minerCharacter = Instantiate(minerCharacterPrefab, player.gameObject.transform);
                minerCharacter.transform.position = minerSpawnPos;
                LocalStateManager.instance.localPlayerCharacter = minerCharacter;

                mineManager.SpawnMine(player.synchronizedPlayerGameData.teamNumber);
                string hash = LobbyManager.instance.GenerateRandomString(20);
                minerCharacter.GetComponent<CharacterControllerBase>().SetHash(hash);

                CMDSpawnPuppetOnClients(player.synchronizedPlayerGameData.matchPtr.matchID, player.GetUserData().username, hash,PuppetType.MINER, minerSpawnPos);
            }

            else if (role == GameRole.WARRIOR)
            {
                Debug.Log("You are warrior");
                gameUI.TurnMinerUI(false);
                gameUI.TurnTownManagerUI(false);
                gameUI.TurnWarriorUI(true);
                gameUI.TurnAdminUI(false);

                worldMapCharacter = Instantiate(worldMapCharacterPrefab, player.transform);
                Vector3 spawnPos = worldMapSpawnPos + (player.synchronizedPlayerGameData.teamNumber == 1 ? worldMapCharacterSpawnOffset : -worldMapCharacterSpawnOffset);
                worldMapCharacter.transform.position = spawnPos;
                string hash = LobbyManager.instance.GenerateRandomString(20);
                worldMapCharacter.GetComponent<CharacterControllerBase>().SetHash(hash);

                LocalStateManager.instance.localPlayerCharacter = worldMapCharacter;

                CameraController.instance.SetBounds(worldMapInstance.GetComponent<BoxCollider2D>().bounds);
                CameraController.instance.SetFollowingPlayer(false);

                CMDSpawnPuppetOnClients(player.synchronizedPlayerGameData.matchPtr.matchID, player.GetUserData().username, hash, PuppetType.WARRIOR, spawnPos);

                //LocalStateManager.instance.localPlayer.transform.position.Set(worldMapSpawnPos.x, worldMapSpawnPos.y, worldMapSpawnPos.z); //= worldMapSpawnPos;
            }

            else if (role == GameRole.ADMIN)
            {
                Debug.Log("You are ADMIN");
                gameUI.TurnMinerUI(false);
                gameUI.TurnTownManagerUI(false);
                gameUI.TurnWarriorUI(false);
                gameUI.TurnAdminUI(true);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDUpdateTransformCharacter(string matchID, string ownerUsername, string hash, Vector3 position, Vector3 scale)
    {
        foreach (PlayerNetworking p in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = p.GetComponent<NetworkIdentity>().connectionToClient;
            string username = p.GetUserData().username;
            if (username != ownerUsername)
            {
                RPCUpdateTransformCharacter(conn, hash, position, scale);
            }
        }
    }

    [TargetRpc]
    private void RPCUpdateTransformCharacter(NetworkConnectionToClient conn, string hash, Vector3 position, Vector3 scale)
    {
        foreach(CharacterControllerBase puppet in puppetsList)
        {
            if (puppet == null) continue;
            if (puppet.GetHash() == hash)
            {
                puppet.gameObject.transform.position = position;
                puppet.gameObject.transform.localScale = scale;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDSpawnPuppetOnClients(string matchID, string ownerUser, string hash, PuppetType puppetType, Vector3 position)
    {

        Debug.LogError("Calling RPC on " + puppetType + " " + position);
        List<PlayerNetworking> playersInMatch = LobbyManager.instance.GetPlayersInMatch(matchID);
        if (playersInMatch == null)
        {
            Debug.LogError("Plaers in match is NULL");
            return;
        }
        foreach (PlayerNetworking p in playersInMatch)
        {
            NetworkConnectionToClient conn = p.GetComponent<NetworkIdentity>().connectionToClient;
            string user = p.GetUserData().username;
            if (ownerUser != user)
            {
                RPCSpawnPuppetOnClients(conn,hash, puppetType, position);
            }
        }
    }

    [Command(requiresAuthority =false)]
    public void CMDDestroyPuppetOnClients(string matchID, string hash)
    {
        foreach(PlayerNetworking p in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = p.GetComponent<NetworkIdentity>().connectionToClient;
            RPCDestroyPuppetOnClients(conn, hash);
        }
    }
    [TargetRpc]
    private void RPCDestroyPuppetOnClients(NetworkConnectionToClient conn, string hash)
    {
        foreach (CharacterControllerBase c in puppetsList)
        {
            if (c.GetHash() == hash)
            {
              // puppetsList.Remove(c);
                Destroy(c.gameObject);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDSpawnArmyPuppetOnClients(string matchID, string ownerUser, string hash, Army army, Vector3 position)
    {
        Debug.LogError("Calling ARMY RPC " + " " + position);
        foreach (PlayerNetworking p in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = p.GetComponent<NetworkIdentity>().connectionToClient;
            string user = p.GetUserData().username;
            if (ownerUser != user)
            {
                RPCSpawnArmyPuppetOnClients(conn, hash, army, position);
            }
        }
    }

    [TargetRpc]
    public void RPCSpawnArmyPuppetOnClients(NetworkConnectionToClient conn, string hash, Army army, Vector3 position)
    {
        Debug.Log("Spawning PUPPET ARMY with " + army.troopTypes.Length + " troops count");
        GameObject character = worldMapArmyPrefab;
        GameObject obj = Instantiate(character, position, character.transform.rotation);
        Debug.Log("Puppet is spawned!");
        CharacterControllerBase charac = obj.GetComponent<CharacterControllerBase>();
        charac.SetIsPuppet(true);
        charac.SetHash(hash);

        Troop[] troops = new Troop[3];
        for (int i = 0; i < army.troopTypes.Length; i++)
        {
            foreach (TroopData data in troopDataArray)
            {
                if (data.troopType == army.troopTypes[i])
                {
                    troops[i] = new Troop();
                    troops[i].data = data;
                    troops[i].count = army.counts[i];
                }
            }
        }
        obj.GetComponent<WorldMapArmyAI>().SetTroopsInArmy(troops);
        obj.GetComponent<WorldMapArmyAI>().SetNoOutline();
        puppetsList.Add(charac);
    }

    [TargetRpc]
    private void RPCSpawnPuppetOnClients(NetworkConnectionToClient conn, string hash, PuppetType puppetType, Vector3 position)
    {
        GameObject character = null;
        switch (puppetType)
        {
            case PuppetType.TOWN_MANAGER: character = townManagerCharacterPrefab; break;
            case PuppetType.MINER: character = minerCharacterPrefab; break;
            case PuppetType.WARRIOR: character = worldMapCharacterPrefab; break;
            case PuppetType.BARBARIAN: character = barbarianPrefab; break;
            case PuppetType.MERCENARY: character = mercenaryPrefab; break;
            case PuppetType.HEAVY_KNIGHT: character = heavyKnightPrefab; break;
            case PuppetType.ARMY: character = worldMapArmyPrefab; break; 
        }
        if (character == null)
        {
            Debug.LogError("RPCSpawnPuppetOnClients character is NULL");
            return;
        }
        GameObject obj = Instantiate(character, position, character.transform.rotation);
        Debug.Log("Puppet is spawned!");
        CharacterControllerBase charac = obj.GetComponent<CharacterControllerBase>();
        charac.SetIsPuppet(true);
        charac.SetHash(hash);
        puppetsList.Add(charac);
    }

    [Command(requiresAuthority = false)]
    public void CMDDoGameOver(string matchID)
    {
        foreach(PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = player.GetComponent<NetworkIdentity>().connectionToClient;
            RPCDoGameOver(conn);
            //LobbyManager.instance.ServerPlayerLeaveMatch(player, matchID);
        }
    }

    [TargetRpc]
    private void RPCDoGameOver(NetworkConnectionToClient conn)
    {
        gameUI.ShowGameOver();
    }

    public void SetLocalCharacter(GameRole role)
    {
        foreach (CharacterControllerBase c in puppetsList)
        {
            if (c is TownManagerController && role == GameRole.TOWN_MANAGER)
            {
                LocalStateManager.instance.localPlayerCharacter = c.gameObject;
            }
            else if (c is MinerController && role == GameRole.MINER)
            {
                LocalStateManager.instance.localPlayerCharacter = c.gameObject;
            }
            else if (c is WorldMapWarriorController && role == GameRole.WARRIOR)
            {
                LocalStateManager.instance.localPlayerCharacter = c.gameObject;
            }
        }
    }

    public static PuppetType ConvertResourceToPuppet(Resource res)
    {
        if (res == Resource.BARBARIAN) return PuppetType.BARBARIAN;
        else if (res == Resource.MERCENARY) return PuppetType.MERCENARY;
        else if (res == Resource.HEAVY_KNIGHT) return PuppetType.HEAVY_KNIGHT;
        return PuppetType.BARBARIAN;
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
            Debug.LogError("Message received " + msg.matchID + " " + msg.resource + " " + msg.amount);
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
                case Resource.GOLD_ORE: gameData.AddGoldOre(msg.amount, msg.teamNumber); break;
                case Resource.BARBARIAN: gameData.AddBarbarians(msg.amount, msg.teamNumber); break;
                case Resource.MERCENARY: gameData.AddMercenaries(msg.amount, msg.teamNumber); break;
                case Resource.HEAVY_KNIGHT: gameData.AddHeavyKnights(msg.amount, msg.teamNumber); break;
            }
            UpdateGameDataAndSync(matchID, gameData);
        }

        // Cheats
        if (Input.GetKey(KeyCode.K))
        {
            PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
            string matchID = player.synchronizedPlayerGameData.matchPtr.matchID;
            int teamNum = player.synchronizedPlayerGameData.teamNumber;
            if (Input.GetKey(KeyCode.M))
            {                
                ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, 10, Resource.FOOD);
                CMDUpdateResource(msg);
                Debug.Log("Added cheated food");
            }
            else if (Input.GetKey(KeyCode.G))
            {
                ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, 10, Resource.GOLD);
                CMDUpdateResource(msg);
                Debug.Log("Added cheated gold");
            }
        }
    }
}
