using Mirror.Examples.Common.Controllers.Tank;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    // Generate and store random ID of destructible object. These objects with corresponding IDs must be spawned on all clients. When on one client
    // it gets destroyed, the RCP call is sent to destroy the same object on other clients
    
    private DestructibleObjectData objectData;

    int currentHealth;
    string matchID;
    bool isDestructionTriggered = false;
    float resourceSpawnRadius = 2f;

    public void SetObjectData(DestructibleObjectData objectData)
    {
        this.objectData = objectData;

        currentHealth = objectData.health;
    }
    public void Damage(int health)
    {
        currentHealth -= health;
    }

    public int GetHealth()
    {
        return currentHealth; 
    }

    public void SpawnResourcesAndDestroy()
    {
        for (int i = 0; i < objectData.resourceCanDrop.Length; i++)
        {
            int mustGet = objectData.resourceCanDropChanceRange[i];
            int got = Random.Range(0, 9);
            if (got <= mustGet)
            {
                Vector2 point = Random.insideUnitCircle * resourceSpawnRadius;
                GameObject obj = Instantiate(objectData.resourceCanDrop[i], transform.position + (Vector3)point, objectData.resourceCanDrop[i].transform.rotation);
            }
        }

        Destroy(gameObject);
    }
    void Awake()
    {
        matchID = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (!isDestructionTriggered && currentHealth <= 0)
        {
            GameManager.instance.mineManager.CMDDestroyDestructibleObject(matchID, transform.position.x.ToString() + transform.position.y.ToString());
            isDestructionTriggered = true;
            
        }
    }
    
}
