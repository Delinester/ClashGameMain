using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingListSelectButtonScript : MonoBehaviour
{
    public BuildingData buildingData;
    public GameUI gameUI;

    private GameData gameData;
    private int teamNum;
    private int goldInPossession = 0;

    private bool isDisabled = false;
    private bool isNotEnoughToBuy = false;
    void Awake()
    {
        gameUI = FindObjectOfType<GameUI>();
        teamNum = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber;
        gameData = LocalStateManager.instance.localGameData;
    }
    // Start is called before the first frame update
    void Start()
    {
        //buildingData = GetComponentInParent<BuildingListEntryScript>().GetBuildingData();
        buildingData = GetComponent<BuildingListEntryScript>().GetBuildingData();
        GetComponent<Button>().onClick.AddListener(delegate () { gameUI.EnterBuidingMode(buildingData); });
        UpdateGoldInteractable(0);
    }

    public void UpdateGoldInteractable(int goldHave)
    {
        if (!isDisabled)
        {
            if (goldHave < buildingData.costGold)
            {
                if (buildingData.buildingName == "TownHall")
                    Debug.Log("No moneh " + goldInPossession + buildingData.costGold);
                GetComponent<Button>().interactable = false;
            }
            else
            {
                if (buildingData.buildingName == "TownHall")
                    Debug.Log("HAVE moneh " + goldInPossession + buildingData.costGold);
                GetComponent<Button>().interactable = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDisabled && buildingData.currentlyBuiltAmount >= buildingData.maxQuantity)
        {
            GetComponent<Button>().interactable = false;
            isDisabled = true;
        }        
    }
}
