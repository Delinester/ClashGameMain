using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField]
    private List<BuildingData> buildings = new List<BuildingData>();

    private Dictionary<string, GameObject> buildingsDictionary = new Dictionary<string, GameObject>();

    public List<BuildingData> GetBuildingsDataList() { return buildings; }
    public GameObject GetBuildingPrefabByName(string name)
    {
        return buildings.Find(x => x.buildingName == name).buildingPrefab;
    }

    [Command(requiresAuthority = false)]
    public void PlaceBuilding(string buildingName, Vector3 position, string matchID, PlayerNetworking owner, string hash)
    {
        List<PlayerNetworking> players = LobbyManager.instance.GetPlayersInMatch(matchID);
        foreach (PlayerNetworking player in players)
        {
            PlaceBuildingClient(player.GetComponent<NetworkIdentity>().connectionToClient, buildingName, position, owner, hash);
        }
       
    }

    [TargetRpc]
    private void PlaceBuildingClient(NetworkConnectionToClient conn, string buildingName, Vector3 position, PlayerNetworking owner, string hash)
    {
        GameObject prefab = GetBuildingPrefabByName(buildingName);
        GameObject obj = Instantiate(prefab, position, prefab.transform.rotation);
        obj.GetComponent<Building>().SetPlayer(owner);
        obj.GetComponent<Building>().SetFunctional(true);
        obj.GetComponent<Building>().SetHash(hash);

        if (!buildingsDictionary.TryAdd(hash, obj))
        {
            Debug.LogError("Error adding building to dictionary");
        }

        GameManager.instance.BakeNavMesh();
        // obj.GetComponent<Building>().owner = owner;
    }

    [Command(requiresAuthority = false)]
    public void CMDDestroyBuilding(string matchID, string hash)
    {
        foreach(PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = player.GetComponent<NetworkIdentity>().connectionToClient;
            RPCDestroyBuilding(conn, hash);
        }
    }

    [TargetRpc]
    private void RPCDestroyBuilding(NetworkConnectionToClient conn, string hash)
    {
        Debug.Log("Destroying building " + hash);
        buildingsDictionary.TryGetValue(hash, out GameObject building);
        buildingsDictionary.Remove(hash);
        Destroy(building);
        StartCoroutine(BakeAfterDelay());
    }

    private IEnumerator BakeAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.BakeNavMesh();
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
