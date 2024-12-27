using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.LogError($"The client with IP {conn.address} connected!");
    }
    
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("Client disconnected!!!");
        foreach (Match m in LobbyManager.instance.matchesList)
        {
            foreach (PlayerNetworking p in m.players)
            {
                if (p.connectionToClient == conn)
                {
                    Debug.Log("Disconnected from match " + m.matchName + " with ID " + m.matchID + " player " + p.GetUserData().username);
                    LobbyManager.instance.ServerPlayerLeaveMatch(p, m.matchID);
                    break;
                }
            }
        }

        base.OnServerDisconnect(conn);
    }
}
