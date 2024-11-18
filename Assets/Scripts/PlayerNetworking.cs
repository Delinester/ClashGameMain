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
    GameManager.GameRole role;
    public PlayerGameData()
    {
        teamNumber = 1;
    }
}
public class PlayerNetworking : NetworkBehaviour
{
    [System.Serializable]
    public class UserData 
    {
        public string username;
    }

    [SyncVar(hook = nameof(OnGameDataUpdate))]
    public PlayerGameData synchronizedPlayerGameData = new PlayerGameData();

    private PlayerGameData localPlayerGameData = new PlayerGameData();


    //public bool isInMatch = false;
    //public Match matchPtr = null;
    public NetworkConnectionToClient clientConnection;

    private UserData localUserData;

    [SyncVar(hook = nameof(OnUserDataUpdate))]
    private UserData syncronizedUserData;

    private void OnUserDataUpdate(UserData _old, UserData _new)
    {
        localUserData = _new;
    }

    private void OnGameDataUpdate(PlayerGameData _old, PlayerGameData _new)
    {
        localPlayerGameData = _new;
    }

    [Command]
    public void AssignUserData(UserData data)
    {
        syncronizedUserData = data;
    }

    [Command]
    public void AssignGameData(PlayerGameData data)
    {
        synchronizedPlayerGameData = data;
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
