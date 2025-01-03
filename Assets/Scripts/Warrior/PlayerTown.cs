using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTown : MonoBehaviour
{
    [SerializeField]
    private GameObject interactionUI;

    private string matchID;
    [SerializeField]
    private int teamNum;

    private bool isTrainingTroop;

    private Canvas canvas;

    public void TrainTroop(TroopData data)
    {
        StartCoroutine(TroopTrainCoroutine(data));
    }

    public bool isTroopTraining()
    {
        return isTrainingTroop;
    }

    private IEnumerator TroopTrainCoroutine(TroopData data)
    {
        isTrainingTroop= true;
        Debug.Log("Started training troop + " + data.troopName);
        yield return new WaitForSeconds(data.trainTimeSeconds);
        ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, 1, data.troopType);
        GameManager.instance.CMDUpdateResource(msg);
        isTrainingTroop = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Town collided with " + collision.name);
        if (collision.gameObject.tag == "Army" && teamNum != collision.GetComponent<WorldMapArmyAI>().GetOwnerTeamNum())
        {
            matchID = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;

            Debug.Log("TOWN COLLIDED WITH ARMY! MatchID: " + matchID);
            WorldMapArmyAI army = collision.GetComponent<WorldMapArmyAI>();
            GameManager.instance.battlesManager.CMDSpawnArmyInTown(matchID, new Army(collision.gameObject.GetComponent<WorldMapArmyAI>().GetTroopsInArmy()), teamNum);

            GameManager.instance.battlesManager.CMDDestroyArmyByHash(matchID, army.GetHash());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerNetworking playerNetworking = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        matchID = playerNetworking.synchronizedPlayerGameData.matchPtr.matchID;
        //teamNum = playerNetworking.synchronizedPlayerGameData.teamNumber;

        canvas = GetComponentInChildren<Canvas>();

        if (teamNum == 2)
        {
            Vector3 pos = interactionUI.transform.position;
            interactionUI.transform.position = new Vector3(pos.x, pos.y + 3f, pos.z);    
        }
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.4f);
        bool isTherePlayer = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                isTherePlayer = true;
                //interactionUI.SetActive(true);
                canvas.enabled = true;
                //interactionUI.GetComponent<Renderer>().enabled = true;
            }
        }

        if (!isTherePlayer && interactionUI.activeSelf)
        {
            //interactionUI.SetActive(false);
            canvas.enabled = false;
            //interactionUI.GetComponent<Renderer>().enabled = false;
        }
    }
}
