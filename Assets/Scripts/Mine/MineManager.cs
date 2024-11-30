using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : NetworkBehaviour
{
    [SerializeField]
    public List<DestructibleObjectData> spawnableObjectsList;

    [SerializeField]
    private GameObject[] mineRoomPrefabs;

    public Vector3 mine1Location = new Vector3(0,6669,0);
    public Vector3 mine2Location = new Vector3(0, -6669, 0);

    [Command(requiresAuthority = false)]
    public void CMDPlaceRandomObject(string matchID, Vector2 minPos, Vector2 maxPos) 
    {
        for (int i = 0; i < spawnableObjectsList.Count; i++) 
        {
            DestructibleObjectData obj = spawnableObjectsList[i];
            int mustGet = Random.Range(0, obj.spawnChanceRange);
            int got = Random.Range(0, obj.spawnChanceRange);
            if (mustGet == got)
            {
                float randX = Random.Range(minPos.x, maxPos.x);
                float randY = Random.Range(minPos.y, maxPos.y);

                int randPrefabIdx = Random.Range(0, obj.prefabs.Length);
                foreach(PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
                {
                    RPCSpawnObjectOnClients(player.GetComponent<NetworkIdentity>().connectionToClient, i, randPrefabIdx, new Vector2(randX, randY));
                }
            }
        }

    }
    [TargetRpc]
    private void RPCSpawnObjectOnClients(NetworkConnectionToClient conn, int listIdx, int prefabIdx, Vector2 pos)
    {
        GameObject prefab = spawnableObjectsList[listIdx].prefabs[prefabIdx];
        Instantiate(prefab, new Vector3(pos.x, pos.y, prefab.transform.position.z), prefab.transform.rotation);
    }

    public Vector3 GetMineLocation(int team)
    {
        return team == 1 ? mine1Location : mine2Location;
    }

    public void GenerateMine()
    {
        for (int i = 0; i < mineRoomPrefabs.Length; i++)
        {
            int randIdx = Random.Range(0, mineRoomPrefabs.Length);
            GameObject room = Instantiate(mineRoomPrefabs[randIdx], mine1Location, mineRoomPrefabs[randIdx].transform.rotation);
            GameObject room2 = Instantiate(mineRoomPrefabs[randIdx], mine2Location, mineRoomPrefabs[randIdx].transform.rotation);
        }
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
