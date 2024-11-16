using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworking : NetworkBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class UserData // Replace with your actual data structure
    {
        public string username;
        // Add other fields as needed
    }

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

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
