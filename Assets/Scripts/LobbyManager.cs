using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

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

    public Match(string matchID, string matchName, PlayerNetworking player, bool passwordProtected, string password = "")
    {
        matchFull = false;
        inMatch = false;
        this.matchID = matchID;
        this.matchName = matchName;
        players.Add(player);
        this.passwordProtected = passwordProtected;
        this.password = password;
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

    [Command(requiresAuthority =false)]
    public void CreateMatch(string matchName, PlayerNetworking player)
    {
        string matchId = GenerateRandomString(10);
       // PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        matchIDs.Add(matchId);
        Match match = new Match(matchId, matchName, player, false);
        matchesList.Add(match);
        player.matchPtr = match;
        Debug.Log("Match with ID " + matchId + " was created and name " + match.matchName);
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
