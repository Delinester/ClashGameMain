using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField]
    private List<BuildingData> buildings = new List<BuildingData>();


    public List<BuildingData> GetBuildingsDataList() { return buildings; }
    public GameObject GetBuildingPrefabByName(string name)
    {
        return buildings.Find(x => x.buildingName == name).buildingPrefab;
    }

    [Command(requiresAuthority = false)]
    public void PlaceBuilding(string buildingName, Vector3 position, string matchID, PlayerNetworking owner)
    {
        List<PlayerNetworking> players = LobbyManager.instance.GetPlayersInMatch(matchID);
        foreach (PlayerNetworking player in players)
        {
            PlaceBuildingClient(player.GetComponent<NetworkIdentity>().connectionToClient, buildingName, position, owner);
        }
       
    }

    [TargetRpc]
    private void PlaceBuildingClient(NetworkConnectionToClient conn, string buildingName, Vector3 position, PlayerNetworking owner)
    {
        GameObject prefab = GetBuildingPrefabByName(buildingName);
        GameObject obj = Instantiate(prefab, position, prefab.transform.rotation);
        obj.GetComponent<Building>().SetPlayer(owner);
        obj.GetComponent<Building>().SetFunctional(true);
        // obj.GetComponent<Building>().owner = owner;
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
