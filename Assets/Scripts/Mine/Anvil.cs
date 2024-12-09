using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Anvil : MonoBehaviour
{
    [SerializeField]
    private int amountToUpgradeRequired = 10;
    [SerializeField]
    private int damageIncrease = 15;
    [SerializeField]
    private GameObject interactionUI;
    [SerializeField]
    private TextMeshProUGUI interactionText;

    private string interactiBaseText = "Press E to UPGRADE";
    private string notEnoughResText = "Minerals need: 10";

    private MinerController minerController;
    private int teamNum;
    string matchID;

    private void Awake()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        minerController = LocalStateManager.instance.localPlayerCharacter.GetComponentInChildren<MinerController>();
        if (!minerController) Debug.LogError("Error accessing Miner Controller: Anvil");
        PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        teamNum = player.synchronizedPlayerGameData.teamNumber;
        matchID = player.synchronizedPlayerGameData.matchPtr.matchID;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1.4f);
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
                interactionUI.SetActive(true);
                bool isEnoughResources = LocalStateManager.instance.localGameData.GetMinerals(teamNum) >= amountToUpgradeRequired;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (isEnoughResources)
                    {
                        ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, -amountToUpgradeRequired, Resource.MINERAL);
                        GameManager.instance.CMDUpdateResource(msg);
                        minerController.IncreaseDamageBy(damageIncrease);
                    }
                    else
                    {
                        interactionText.text = notEnoughResText;
                    }
                }
                
            }
        }

        if (!isTherePlayer && interactionUI.activeSelf)
        {
            interactionText.text = interactiBaseText;
            interactionUI.SetActive(false);
        }
    }
}
