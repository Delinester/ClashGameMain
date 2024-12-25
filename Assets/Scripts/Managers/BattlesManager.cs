using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlesManager : NetworkBehaviour
{ 
    private Dictionary<string, WorldMapArmyAI> armiesHashTable = new Dictionary<string, WorldMapArmyAI>();

    private string matchID;

    [SerializeField]
    private float baseBattleTimeSecs = 6f;
    [SerializeField]
    private float minBattleTimeSecs = 2f;

    [Client]
    public void AddNewArmy(WorldMapArmyAI army)
    {
        string hash = army.GetHash();
        if (!armiesHashTable.TryAdd(hash, army))
        {
            Debug.LogError("Error inserting army into HashTable (should be fiiiine)");
        }
    }

    [Client]
    public void DestroyArmy(string hash)
    {
        if (armiesHashTable.TryGetValue(hash, out WorldMapArmyAI army))
        {
            Destroy(army.gameObject);
            armiesHashTable.Remove(hash);
        }
    }

    [Command(requiresAuthority =false)]
    public void CMDDestroyArmyByHash(string matchID, string hash)
    {
        foreach (PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = player.GetComponent<NetworkIdentity>().connectionToClient;
            RPCDestroyArmyByHash(conn, hash);
        }
    }

    [Command(requiresAuthority =false)]
    public void CMDMakeArmySelectable(string matchID, string hash)
    {
        foreach (PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = player.GetComponent<NetworkIdentity>().connectionToClient;
            RPCMakeArmySelectable(conn, hash);
        }
    }

    [TargetRpc]
    private void RPCMakeArmySelectable(NetworkConnectionToClient conn, string hash)
    {
        if (!armiesHashTable.TryGetValue(hash, out WorldMapArmyAI army))
        {
            Debug.LogError("Cannot make army selectable");
        }
        army.SetIsInCombat(false);
    }

    [TargetRpc]
    public void RPCDestroyArmyByHash(NetworkConnectionToClient conn, string hash)
    {
        DestroyArmy(hash);
    }

    [Client]
    private void CalculateWinnerAndDestroy(WorldMapArmyAI army1, WorldMapArmyAI army2)
    {

        matchID = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;
        int army1points = army1.GetArmyPoints();
        int army2points = army2.GetArmyPoints();

        int goldReward = 0, foodReward = 0, teamRewared = 0;

        if (army1points > army2points)
        {
            goldReward = army2.GetArmyGoldCost() / 10;
            foodReward = army2.GetArmyFoodCost() / 10;
            teamRewared = army1.GetOwnerTeamNum();
            CMDDestroyArmyByHash(matchID, army2.GetHash());
            CMDMakeArmySelectable(matchID, army1.GetHash());
        }
        else if (army2points > army1points)
        {
            goldReward = army1.GetArmyGoldCost() / 10;
            foodReward = army1.GetArmyFoodCost() / 10;
            teamRewared = army2.GetOwnerTeamNum();
            CMDDestroyArmyByHash(matchID, army1.GetHash());
            CMDMakeArmySelectable(matchID, army2.GetHash());
        }
        else
        {
            CMDDestroyArmyByHash(matchID, army1.GetHash());
            CMDDestroyArmyByHash(matchID, army2.GetHash());
        }

        ResourceUpdateMsg msgGold = new ResourceUpdateMsg(matchID, teamRewared, goldReward, Resource.GOLD);
        ResourceUpdateMsg msgFood = new ResourceUpdateMsg(matchID, teamRewared, foodReward, Resource.FOOD);
        GameManager.instance.CMDUpdateResource(msgGold);
        GameManager.instance.CMDUpdateResource(msgFood);
    }

    public void EngageInBattle(WorldMapArmyAI army1, WorldMapArmyAI army2)
    {
        float army1Points = army1.GetArmyPoints(), army2Points = army2.GetArmyPoints();
        float dominance1Army = army1Points / army2Points, dominance2Army = army2Points / army1Points;
        float battleDuration = Mathf.Max(baseBattleTimeSecs / Mathf.Max(dominance1Army, dominance2Army), minBattleTimeSecs);
        Debug.Log("Battle between " + army1.GetHash() + " with points " + army1Points + " and " + army2.GetHash() + " with points " + army2Points);
        StartCoroutine(ArmyBattleCoroutine(battleDuration, army1, army2));

    }

    [Command(requiresAuthority =false)]
    public void CMDSpawnArmyInTown(string matchID, Army army, int teamNumber)
    {
        Debug.LogError("CMD SpawnArmyInTown Initiated!");
        foreach (PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = player.GetComponent<NetworkIdentity>().connectionToClient;
            if (player.synchronizedPlayerGameData.role == GameRole.TOWN_MANAGER && player.synchronizedPlayerGameData.teamNumber == teamNumber)
            {
                Debug.LogError("Requested ARMY spawn in town for " + player.GetUserData().username + " in team " + teamNumber);
                RPCSpawnArmyInTown(conn, army, teamNumber);
            }
        }
    }

    [TargetRpc]
    private void RPCSpawnArmyInTown(NetworkConnectionToClient conn, Army army, int teamNumber)
    {        
        for (int i = 0; i < army.counts.Length; i++)
        {
            GameObject troopPrefab = GameManager.instance.GetTroopPrefab(army.troopTypes[i]);
            if (troopPrefab == null)
            {
                Debug.Log("Troop prefab is null!");
            }
            for (int j = 0; j < army.counts[i]; j++)
            {
                string puppetHash = LobbyManager.instance.GenerateRandomString(20);
                Debug.Log("Loop of army spawning!");
                ArmySpawnBounds spawnBounds = GameManager.instance.GetTownArmySpawnBounds(teamNumber);
                Collider2D[] bounds = spawnBounds.GetComponents<Collider2D>();
                Bounds bound = bounds[Random.Range(0, bounds.Length)].bounds;
                float randX = Random.Range(bound.min.x, bound.max.x);
                float randY = Random.Range(bound.min.y, bound.max.y);
                Vector2 spawnPos = new Vector2(randX, randY);

                GameObject troop = Instantiate(troopPrefab, spawnPos, troopPrefab.transform.rotation);
                troop.GetComponent<CharacterControllerBase>().SetHash(puppetHash);
                string username = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().GetUserData().username;

                

                Debug.Log("RPC Spawn army in Town called on " + matchID);
                string match_id = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;
                GameManager.instance.CMDSpawnPuppetOnClients(match_id, username, puppetHash, GameManager.ConvertResourceToPuppet(army.troopTypes[i]), spawnPos);
            }
        }
    }

    private IEnumerator ArmyBattleCoroutine(float time, WorldMapArmyAI army1, WorldMapArmyAI army2)
    {
        yield return new WaitForSeconds(time);
        CalculateWinnerAndDestroy(army1, army2);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
