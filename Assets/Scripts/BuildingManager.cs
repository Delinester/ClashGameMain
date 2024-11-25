using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void PlaceBuilding(string buildingName, Vector3 position)
    {
        Debug.LogError("Place building is called!!!");
        GameObject prefab = GetBuildingPrefabByName(buildingName);
        GameObject obj = Instantiate(prefab, position, prefab.transform.rotation);
        NetworkServer.Spawn(obj);
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
