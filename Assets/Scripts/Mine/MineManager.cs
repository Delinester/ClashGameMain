using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public Vector2Int position = new Vector2Int();
                                    // Up    Down   Left   Right
    public bool[] jammedPassages = { false, false, false, false };

    public void SetJammedUp(bool isJammed) { jammedPassages[0] = isJammed; }
    public void SetJammedDown(bool isJammed) { jammedPassages[1] = isJammed; }
    public void SetJammedLeft(bool isJammed) { jammedPassages[2] = isJammed; }
    public void SetJammedRight(bool isJammed) { jammedPassages[3] = isJammed; }

    public bool isJammedUp() { return jammedPassages[0]; }
    public bool isJammedDown() { return jammedPassages[1]; }
    public bool isJammedLeft() { return jammedPassages[2]; }
    public bool isJammedRight() { return jammedPassages[3]; }

    public Cell(int x, int y)
    {
        position.x = x;
        position.y = y;
        for (int i = 0; i < 4; i++)
        {
            jammedPassages[i] = (Random.Range(0, 2) == 0) ? false : true;
        }
    }

    public Cell() { }
}

public class MineManager : NetworkBehaviour
{
    [SerializeField]
    public List<DestructibleObjectData> spawnableObjectsList;
    
    [SerializeField]
    private GameObject[] mineRoomPrefabs;

    public Vector3 mine1Location = new Vector3(0,6669,0);
    public Vector3 mine2Location = new Vector3(0, -6669, 0);

    List<Cell> mineMap1 = new List<Cell>();
    List<Cell> mineMap2 = new List<Cell>();

    private float roomWidth = 21.62693f;
    private float roomHeight = 13.77677f;

    private const int maxRoomsInMine = 15;

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

    private Cell GetNeighbourUp(Cell cell, int teamNum)
    {
        List<Cell> board = teamNum == 1 ? mineMap1 : mineMap2;
        int cellX = cell.position.x;
        int cellY = cell.position.y;
        foreach(Cell c in board) 
        {
            if (c.position.x == cell.position.x && c.position.y == cell.position.y + 1) return c;
        }
        return null;
    }
    private Cell GetNeighbourDown(Cell cell, int teamNum)
    {
        List<Cell> board = teamNum == 1 ? mineMap1 : mineMap2;
        int cellX = cell.position.x;
        int cellY = cell.position.y;
        foreach (Cell c in board)
        {
            if (c.position.x == cell.position.x && c.position.y == cell.position.y - 1) return c;
        }
        return null;
    }
    private Cell GetNeighbourLeft(Cell cell, int teamNum)
    {
        List<Cell> board = teamNum == 1 ? mineMap1 : mineMap2;
        int cellX = cell.position.x;
        int cellY = cell.position.y;
        foreach (Cell c in board)
        {
            if (c.position.y == cell.position.y && c.position.x == cell.position.x - 1) return c;
        }
        return null;
    }
    private Cell GetNeighbourRight(Cell cell, int teamNum)
    {
        List<Cell> board = teamNum == 1 ? mineMap1 : mineMap2;
        int cellX = cell.position.x;
        int cellY = cell.position.y;
        foreach (Cell c in board)
        {
            if (c.position.y == cell.position.y && c.position.x == cell.position.x + 1) return c;
        }
        return null;
    }

    // will be called on client to not stress the server
    private void GenerateMineMap(Cell cell, int teamNum)
    {
        List<Cell> board = teamNum == 1 ? mineMap1 : mineMap2;
        if (cell == null)
        {
            cell = new Cell(0, 0);
            board.Add(cell);
        }
        if (!cell.isJammedUp())
        {
            Cell neighborUp = GetNeighbourUp(cell, teamNum);
            if (neighborUp != null) neighborUp.SetJammedDown(false);
            else
            {
                if (board.Count >= maxRoomsInMine) cell.SetJammedUp(true);
                else
                {
                    Cell newUpperCell = new Cell(cell.position.x, cell.position.y + 1);
                    newUpperCell.SetJammedDown(false);
                    board.Add(newUpperCell);
                    GenerateMineMap(newUpperCell, teamNum);
                }
            }
        }

        if (!cell.isJammedDown())
        {
            Cell neighborDown = GetNeighbourDown(cell, teamNum);
            if (neighborDown != null) neighborDown.SetJammedUp(false);
            else
            {
                if (board.Count >= maxRoomsInMine) cell.SetJammedDown(true);
                else
                {
                    Cell newDownCell = new Cell(cell.position.x, cell.position.y - 1);
                    newDownCell.SetJammedUp(false);
                    board.Add(newDownCell);
                    GenerateMineMap(newDownCell, teamNum);
                }
            }
        }

        if (!cell.isJammedLeft())
        {
            Cell neighborLeft = GetNeighbourLeft(cell, teamNum);
            if (neighborLeft != null) neighborLeft.SetJammedRight(false);
            else
            {
                if (board.Count >= maxRoomsInMine) cell.SetJammedLeft(true);
                else
                {
                    Cell newLeftCell = new Cell(cell.position.x - 1, cell.position.y);
                    newLeftCell.SetJammedRight(false);
                    board.Add(newLeftCell);
                    GenerateMineMap(newLeftCell, teamNum);
                }
            }
        }

        if (!cell.isJammedRight())
        {
            Cell neighborRight = GetNeighbourRight(cell, teamNum);
            if (neighborRight != null) neighborRight.SetJammedLeft(false);
            else
            {
                if (board.Count >= maxRoomsInMine) cell.SetJammedRight(true);
                else
                {
                    Cell newRightCell = new Cell(cell.position.x + 1, cell.position.y);
                    newRightCell.SetJammedLeft(false);
                    board.Add(newRightCell);
                    GenerateMineMap(newRightCell, teamNum);
                }
            }
        }
    }

    public void SpawnMine(int teamNum)
    {
        GenerateMineMap(null, teamNum);
        List<Cell> board = teamNum == 1 ? mineMap1 : mineMap2;
        foreach(Cell c in board)
        {
            int randRoomIdx = Random.Range(0, mineRoomPrefabs.Length);
            //BoxCollider2D collider = mineRoomPrefabs[randRoomIdx].GetComponentInChildren<RoomBoundary>().gameObject.GetComponent<BoxCollider2D>();
            //if (collider == null) Debug.LogError("Collider is null");
            //roomWidth = collider.bounds.max.x - collider.bounds.min.x;
            //roomHeight = collider.bounds.max.y - collider.bounds.min.y;
            
            //Vector2 roomPos = (Vector2)GetMineLocation(teamNum) + c.position * new Vector2(roomWidth, roomHeight);
            //Debug.Log("Room pos " + roomPos + "Cell pos is " + c.position + "Room WH is " + roomWidth + " " + roomHeight + " and bounds are " + collider.bounds.max + "and min " + collider.bounds.min);

            CMDSpawnMineRoom(LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID, c, randRoomIdx);
            Debug.Log("Cell111  + " + c.position);
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDSpawnMineRoom(string matchID, Cell cell, int prefabIdx)
    {
        foreach (PlayerNetworking player in LobbyManager.instance.GetPlayersInMatch(matchID))
        {
            NetworkConnectionToClient conn = player.GetComponent<NetworkIdentity>().connectionToClient;
            RPCSpawnMineRoom(conn, cell, prefabIdx);
        }
    }

    [TargetRpc]
    private void RPCSpawnMineRoom(NetworkConnectionToClient conn, Cell cell, int prefabIdx)
    {
        Vector2 minePosBase = (Vector2)GetMineLocation(LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber);

        Vector2 roomPos = minePosBase + cell.position * new Vector2(roomWidth, roomHeight);
        GameObject room = Instantiate(mineRoomPrefabs[prefabIdx], roomPos, mineRoomPrefabs[prefabIdx].transform.rotation);
        room.GetComponent<MineInteriorGenerator>().InitRoom(cell.jammedPassages, (cell.position.x == 0 && cell.position.y == 0) ? true : false);

        //BoxCollider2D collider = room.GetComponentInChildren<RoomBoundary>().gameObject.GetComponent<BoxCollider2D>();
        //if (collider == null) Debug.LogError("Collider is null");
        //roomWidth = collider.bounds.max.x - collider.bounds.min.x;
        //roomHeight = collider.bounds.max.y - collider.bounds.min.y;
        Debug.Log("Room pos " + roomPos + "Cell pos is " + cell.position + "Room WH is " + roomWidth + " " + roomHeight);

        // GameObject room2 = Instantiate(mineRoomPrefabs[prefabIdx], pos, mineRoomPrefabs[prefabIdx].transform.rotation);
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
