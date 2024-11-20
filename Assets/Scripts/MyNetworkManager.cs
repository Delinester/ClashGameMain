using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("Client disconnected!!!");
        foreach (Match m in LobbyManager.instance.matchesList)
        {
            foreach (PlayerNetworking p in m.players)
            {
                if (p.connectionToClient == conn)
                {
                    Debug.Log("Disconnected from match " + m.matchName + " with ID " + m.matchID);
                    LobbyManager.instance.ServerPlayerLeaveMatch(p, m.matchID);
                    break;
                }
            }
        }

        base.OnServerDisconnect(conn);
    }
}
