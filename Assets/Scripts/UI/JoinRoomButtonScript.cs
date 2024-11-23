using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomButtonScript : MonoBehaviour
{
    public LobbyUI lobbyUI;
    // Start is called before the first frame update
    void Start()
    {
        lobbyUI = FindObjectOfType<LobbyUI>();
    }

    public void OnJoinClick(RoomListEntry entry)
    {
        lobbyUI.OnJoinRoomPressed(entry);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
