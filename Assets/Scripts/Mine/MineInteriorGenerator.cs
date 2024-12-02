using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineInteriorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnersInRoom;

    [SerializeField]
    private GameObject backToShackTeleporter;

    public bool isFirstRoom = false;

                                    // Up    Down   Left   Right
    public bool[] jammedPassages = { false, false, false, false };
    public GameObject[] jammedPassagesObjects;
    // Start is called before the first frame update
    void Start()
    {
        // SPAWN ON ALL CLIENTS!!!
        if (isFirstRoom)
        {
            // SPAWN LADDER

        }
        foreach (GameObject spawner in spawnersInRoom)
        {
            Collider2D collider = spawner.GetComponent<Collider2D>();
            Vector2 minPos = collider.bounds.min;
            Vector2 maxPos = collider.bounds.max;
            string matchID = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;

            for (int i = 0; i < Random.Range(3, 7); i++)
            {
                Debug.Log("Spawn Request initiated!");
                GameManager.instance.mineManager.CMDPlaceRandomObject(matchID, minPos, maxPos);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
