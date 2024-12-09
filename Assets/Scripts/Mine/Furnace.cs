using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : MonoBehaviour
{
    [SerializeField]
    private int amountToForgeRequired = 5;
    [SerializeField]
    private float forgingTime = 5;
    [SerializeField]
    private GameObject interactionUI;
    [SerializeField]
    private TextMeshProUGUI interactionText;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private GameObject objectWithSlider;

    private string interactiBaseText = "Press E to refine\nGOLD";
    private string notEnoughResText = "Gold ore need: 5";

    private bool isWorking = false;

    private MinerController minerController;
    private int teamNum;
    string matchID;
    // Start is called before the first frame update
    void Start()
    {
        minerController = LocalStateManager.instance.localPlayerCharacter.GetComponentInChildren<MinerController>();
        if (!minerController) Debug.LogError("Error accessing Miner Controller: Furnace");
        PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        teamNum = player.synchronizedPlayerGameData.teamNumber;
        matchID = player.synchronizedPlayerGameData.matchPtr.matchID;
    }

    private IEnumerator ForgeGold()
    {
        objectWithSlider.SetActive(true);
        isWorking = true;
        float elapsedTime = 0f;
        float startValue = 0f;
        float endValue = 1f; 

        while (elapsedTime < forgingTime)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, endValue, elapsedTime / forgingTime);
            yield return null; 
        }

        slider.value = endValue;

        ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, amountToForgeRequired, Resource.GOLD);
        GameManager.instance.CMDUpdateResource(msg);
        isWorking = false;
        objectWithSlider.SetActive(false);
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
                bool isEnoughResources = LocalStateManager.instance.localGameData.GetGoldOre(teamNum) >= amountToForgeRequired;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (isEnoughResources && !isWorking)
                    {
                        ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, -amountToForgeRequired, Resource.GOLD_ORE);
                        GameManager.instance.CMDUpdateResource(msg);
                        StartCoroutine(ForgeGold());
                    }
                    else if (!isEnoughResources)
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
