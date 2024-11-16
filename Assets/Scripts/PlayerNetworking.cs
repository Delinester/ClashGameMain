using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetworking : NetworkBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class UserData // Replace with your actual data structure
    {
        public string username;
        // Add other fields as needed
    }
    public bool isInMatch = false;
    public Match matchPtr = null;

    private UserData localUserData;

    [SyncVar(hook = nameof(OnUserDataUpdate))]
    private UserData syncronizedUserData;

    private void OnUserDataUpdate(UserData _old, UserData _new)
    {
        localUserData = _new;
    }

    [Command]
    public void AssignUserData(UserData data)
    {
        syncronizedUserData = data;
    }

    public UserData GetUserData()
    {
        return syncronizedUserData;
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
