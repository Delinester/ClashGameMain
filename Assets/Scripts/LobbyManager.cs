using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using Unity.VisualScripting;

[System.Serializable]
public class Match
{
    public string matchID;
    public bool inMatch;
    public bool matchFull;
    public bool passwordProtected;
    public string password;
    public string matchName;
    public List<PlayerNetworking> players = new List<PlayerNetworking>();

    public Match(string matchID, string matchName, bool passwordProtected, string password = "")
    {
        matchFull = false;
        inMatch = false;
        this.matchID = matchID;
        this.matchName = matchName;
        //players.Add(player);
        this.passwordProtected = passwordProtected;
        this.password = password;
    }

    public Match(Match match)
    {
        this.matchID = match.matchID;   
        this.matchName = match.matchName;   
        this.inMatch = match.inMatch;
        this.matchFull = match.matchFull;
        this.passwordProtected = match.passwordProtected;
        this.password = match.password;
        foreach(PlayerNetworking p in match.players)
        {
            this.players.Add(p);
        }
    }

    public Match() { }
}

public class LobbyManager : NetworkBehaviour
{
    public readonly SyncList<Match> matchesList = new SyncList<Match>();
    public readonly SyncList<string> matchIDs = new SyncList<string>();
    // Start is called before the first frame update
    public static LobbyManager instance = null;

    [SerializeField]
    private LobbyUI lobbyUI;

    [Server]
    private void PlayerLeaveMatch(PlayerNetworking player, string matchID)
    {
        if (matchIDs.Contains(matchID))
        {
            Match matchPtr = null;
            foreach (Match match in matchesList) 
            {
                if (match.matchID == matchID) matchPtr = match;
            }
            matchPtr.players.Remove(player);
            PlayerGameData gameData = player.GetGameData();
            gameData.isInMatch = false;
            //player.synchronizedPlayerGameData = gameData;
            player.ServerAssignGameData(gameData);

            Match dummy = new Match();
            matchesList.Add(dummy);
            matchesList.Remove(dummy);

            if (matchPtr.players.Count == 0)
            {
                matchIDs.Remove(matchID);
                matchesList.Remove(matchPtr);
                return;
            }
            RPCSendWaitingRoomUpdateForClients(matchPtr.matchID);
        }
    }

    [Server]
    public void ServerPlayerLeaveMatch(PlayerNetworking player, string matchID)
    {
        PlayerLeaveMatch(player, matchID);
    }

    [Command(requiresAuthority = false)]
    public void CMDPlayerLeaveMatch(PlayerNetworking player, string matchID)
    {
        PlayerLeaveMatch(player, matchID);
    }


    [Command(requiresAuthority =false)]
    public void CreateMatch(string matchName, PlayerNetworking player)
    {
        string matchId = GenerateRandomString(10);
       // PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        matchIDs.Add(matchId);
        Match match = new Match(matchId, matchName, false);
        matchesList.Add(match);
        //player.matchPtr = match;
        Debug.Log("Match with ID " + matchId + " was created and name " + match.matchName);

        JoinAfterCreate(player.gameObject.GetComponent<NetworkIdentity>().connectionToClient, matchId, player);
    }

    [TargetRpc]
    private void JoinAfterCreate(NetworkConnectionToClient conn, string matchID, PlayerNetworking player)
    {
        CMDJoinMatch(matchID, player);
    }
        
    [Command(requiresAuthority = false)]
    public void CMDJoinMatch(string matchID, PlayerNetworking player)
    {
        if (matchIDs.Contains(matchID))
        {
            Match match = null;
            foreach (Match m in matchesList)
            {
                if (m.matchID == matchID)
                {
                    match = m;
                    m.players.Add(player);
                    PlayerGameData gameData = new PlayerGameData(player.GetGameData());
                    gameData.matchPtr = match;
                    gameData.isInMatch = true;

                    // player.synchronizedPlayerGameData = gameData;
                    player.ServerAssignGameData(gameData);

                    Debug.LogError("Client team is " + player.synchronizedPlayerGameData.teamNumber);
                    RPCSendWaitingRoomUpdateForClients(match.matchID);
                    return;
                }                
            }

            if (match == null) { Debug.LogError("Match is null in joining!"); }
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDSendWaitingRoomUpdateRPC(Match match)
    {
        RPCSendWaitingRoomUpdateForClients(match.matchID);
    }
    
    [Server]
    public void ServerUpdateMatchPlayerData(PlayerNetworking player, PlayerGameData newData)
    {
        for (int i =0; i < matchesList.Count; i++)
        {
            Match m = matchesList[i];
            if (m.matchID == player.synchronizedPlayerGameData.matchPtr.matchID)
            {
                foreach (PlayerNetworking p in m.players)
                {
                    if (p.GetUserData().username == player.GetUserData().username)
                    {
                        p.synchronizedPlayerGameData = new PlayerGameData(newData);
                        matchesList[i] = new Match(m);
                        Debug.LogError("Update match for " + p.GetUserData().username + " to TEAM " + p.synchronizedPlayerGameData.teamNumber);
                    }
                }
            }
        }
    }
    [Command]
    public void CMDUpdateMatchPlayerData(PlayerNetworking player, PlayerGameData newData)
    {
        ServerUpdateMatchPlayerData(player, newData);
    }

    [Server]
    public void RPCSendWaitingRoomUpdateForClients(string matchID)
    {
        
        Match match = null;
        foreach (Match m in matchesList)
        {
            if (m.matchID == matchID)
            {
                match = m;
                Debug.LogError("TEAM ON SERVER IS " + m.players[0].synchronizedPlayerGameData.teamNumber);
            }
        }
        foreach (PlayerNetworking p in match.players)
        {
            NetworkConnectionToClient clientConn = p.gameObject.GetComponent<NetworkIdentity>().connectionToClient;
            // TEST CODE
            PlayerGameData gameData = new PlayerGameData(p.synchronizedPlayerGameData);
            Debug.LogError("GAME DATA ON RPC IS " + gameData.teamNumber);
            gameData.matchPtr = match;
            p.ServerAssignGameData(gameData);
            //
            if (clientConn == null) Debug.LogError("Client conn is null on " + p.GetUserData().username);
            Debug.LogError("TEAM NUM BEFORE RPC CALL ON SERVER IS " + match.players[0].synchronizedPlayerGameData.teamNumber);
            JoinMatch_Client(clientConn, new Match(match));
        }
    }

    [TargetRpc]
    private void JoinMatch_Client(NetworkConnectionToClient conn, Match match)
    {
        Debug.Log("Team in MATCH after RPC AFTER IS " + match.players[0].synchronizedPlayerGameData.teamNumber);
        lobbyUI = FindObjectOfType<LobbyUI>();
        Debug.Log("Initiated joining!!! and match is " + (match != null ? "GOOD" : "BAAAAD"));
        lobbyUI.OpenWaitingRoom();
        lobbyUI.UpdateWaitingRoom(match);
    }

    private string GenerateRandomString(int length)
    {
        string allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] randChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            randChars[i] = allowed[Random.Range(0, allowed.Length)];
        }
        return new string(randChars);
    }

    private void Start()
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
    //void Start()
    //{
    //    //lobbyUI = FindObjectOfType<LobbyUI>();
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
