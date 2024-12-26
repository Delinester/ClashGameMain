using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerGameData
{
    public Match matchPtr;
    public bool isInMatch;
    public int teamNumber;
    public GameRole role;
    public PlayerGameData()
    {
        this.teamNumber = 1;
    }

    public PlayerGameData(PlayerGameData copy)
    {
        this.teamNumber = copy.teamNumber;
        this.matchPtr = copy.matchPtr;
        this.isInMatch = copy.isInMatch;
        this.role = copy.role;
    }
}
public class PlayerNetworking : NetworkBehaviour
{
    [System.Serializable]
    public class UserData
    {
        public string username;
        public string name;
        public string surname;
        public string gender;
        public string b_date;
        public int age;
        public string address;
        public string email;
        public string password;
        public int icon_id;
    }

    [SyncVar(hook = nameof(OnGameDataUpdate))]
    public PlayerGameData synchronizedPlayerGameData = new PlayerGameData();
        
    public PlayerGameData localPlayerGameData = new PlayerGameData();


    //public bool isInMatch = false;
    //public Match matchPtr = null;
    public NetworkConnectionToClient clientConnection;

    private UserData localUserData = new UserData();

    [SyncVar(hook = nameof(OnUserDataUpdate))]
    private UserData syncronizedUserData = new UserData();

    private void OnUserDataUpdate(UserData _old, UserData _new)
    {
        localUserData = _new;
    }

    private void OnGameDataUpdate(PlayerGameData _old, PlayerGameData _new)
    {
        //localPlayerGameData.matchPtr = _new.matchPtr;
        //localPlayerGameData.teamNumber = _new.teamNumber;
        //localPlayerGameData.isInMatch = _new.isInMatch;
        //localPlayerGameData.role = _new.role;
        localPlayerGameData = new PlayerGameData(_new);
    }

    public void CopyGameData(PlayerGameData data)
    {

    }

    [Command]
    public void AssignUserData(UserData data)
    {
        syncronizedUserData = data;
    }

    [Server]
    public void ServerAssignGameData(PlayerGameData data)
    {
        PlayerGameData newGameData = new PlayerGameData(data);
        synchronizedPlayerGameData = newGameData;
        LobbyManager.instance.ServerUpdateMatchPlayerData(this, data);
        //Debug.LogError("SYNC BECOMES TEAM " + synchronizedPlayerGameData.teamNumber);
    }

    [Command]
    public void CMDAssignGameData(PlayerGameData data)
    {
        ServerAssignGameData(data);
    }

    public UserData GetUserData()
    {
        return syncronizedUserData;
    }

    public PlayerGameData GetGameData()
    {
        return localPlayerGameData;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    override public void OnStartClient() 
    {

        Debug.Log("OnConnected is called!");
        if (isLocalPlayer)
        {
            LocalStateManager.instance.localPlayer = gameObject;
            clientConnection = GetComponent<NetworkIdentity>().connectionToClient;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
