using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineInteriorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnersInRoom;

    [SerializeField]
    private GameObject backToShackTeleporter;
    [SerializeField]
    private int maxObjectsInRoom = 5;

    public bool isFirstRoom = false;


                                    // Up    Down   Left   Right
    public bool[] jammedPassages = { false, false, false, false };
    public GameObject[] jammedPassagesObjects;

    public void InitRoom(bool[] jammedPassages, bool isFirst)
    {
        this.jammedPassages = jammedPassages;
        for (int i = 0; i < 4; i++)
        {
            if (!jammedPassages[i]) jammedPassagesObjects[i].SetActive(false);
        }
        // SPAWN ON ALL CLIENTS!!!
        if (isFirst)
        {
            isFirstRoom = isFirst;
            // SPAWN LADDER

        }
        List<DestructibleObjectData> destructibleObjectsList = GameManager.instance.mineManager.GetDestructibleObjectsData();
        for (int i =0; i < maxObjectsInRoom; i++)
        {
            int spawnerIdx = Random.Range(0, spawnersInRoom.Length);
            Collider2D collider = spawnersInRoom[spawnerIdx].GetComponent<Collider2D>();
            Vector2 minPos = collider.bounds.min;
            Vector2 maxPos = collider.bounds.max;
            string matchID = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;
            Debug.Log("Initiated object spawn");
            for (int j = 0; j < destructibleObjectsList.Count; j++)
            {
                DestructibleObjectData obj = destructibleObjectsList[j];
                int mustGet = obj.spawnChanceRange;
                int got = Random.Range(0, 9);
                if (got <= mustGet)
                {
                    bool isOccupied = false;
                    int currentTries = 0;
                    float randX = 0, randY = 0;
                    do
                    {
                        randX = Random.Range(minPos.x, maxPos.x);
                        randY = Random.Range(minPos.y, maxPos.y);
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(randX, randY), 5f);
                        foreach (Collider2D coll in colliders)
                        {
                            if (coll.gameObject.tag == "Destructible")
                            {
                                isOccupied = true;
                                currentTries++;
                                if (currentTries == 3) break;
                                continue;
                            }
                        }
                    } while (isOccupied);
                    if (currentTries == 3) 
                    {
                        Debug.Log("Tries exceeded");
                        continue;
                    }
                    int randPrefabIdx = Random.Range(0, obj.prefabs.Length);

                    GameManager.instance.mineManager.CMDPlaceRandomObject(matchID, j, randPrefabIdx, new Vector2(randX, randY));
                }
            }

            //for (int i = 0; i < Random.Range(3, 7); i++)
            //{
            //    //Debug.Log("Spawn Request of mine room interior initiated!");
            //    GameManager.instance.mineManager.CMDPlaceRandomObject(matchID, minPos, maxPos);
            //}
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
