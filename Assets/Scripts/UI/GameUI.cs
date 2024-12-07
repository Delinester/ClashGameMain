using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror.Examples.Common.Controllers.Player;

public class GameUI : MonoBehaviour
{
    [Header("Building Menu")]
    [SerializeField]
    private GameObject buildingViewPortContent;

    [SerializeField]
    private GameObject buildingListEntryPrefab;

    ////////////////////////////////////////////////////////

    [Header("Town Resources Tab")]
    [SerializeField]
    private TextMeshProUGUI goldAmountText;
    [SerializeField]
    private TextMeshProUGUI foodAmountText;
    [SerializeField]
    private TextMeshProUGUI mineralsAmountText;


    ////////////////////////////////////////////////////////
    [Header("Miner tab")]
    [SerializeField]
    private TextMeshProUGUI goldOreAmountText;
    [SerializeField]
    private TextMeshProUGUI mineralOreAmountText;
    //////////////////////////////////////////////

    private bool isInBuildingMode = false;
    private GameObject currentBuildingObject;
    private BuildingData currentBuildingData;
    private Camera mainCamera;
    private PlayerNetworking player;
    private CharacterControllerBase playerController;
    private GameObject playerCharacter;

    private GameRole currentPlayerRole;

    [Header("Misc")]
    [SerializeField]
    private GameObject townManagerUI;
    [SerializeField]
    private GameObject minerUI;

    [SerializeField]
    private Animator buildingMenuAnimator;
    private bool isBuildingMenuUp = false;

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        goldAmountText.text = "0";
        foodAmountText.text = "0";
        mineralsAmountText.text = "0";
        player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        currentPlayerRole = player.synchronizedPlayerGameData.role;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (BuildingData building in GameManager.instance.buildingManager.GetBuildingsDataList())
        {
            GameObject obj = Instantiate(buildingListEntryPrefab, buildingViewPortContent.transform);
            obj.GetComponent<BuildingListEntryScript>().SetBuildingData(building);
        }
        playerCharacter = player.gameObject.GetComponentInChildren<CharacterControllerBase>().gameObject;

        playerController = player.gameObject.GetComponentInChildren<CharacterControllerBase>();
    }
   
    public void EnterBuidingMode(BuildingData building)
    {
        if (isInBuildingMode && currentBuildingObject != null) Destroy(currentBuildingObject);
        currentBuildingData = building;
        isInBuildingMode = true;
        currentBuildingObject = Instantiate(building.buildingPrefab);
        currentBuildingObject.GetComponent<Building>().SetBuildingMode(true);
    }

    public void SlideBuildingMenu()
    {
        if (isBuildingMenuUp)
        {
            buildingMenuAnimator.SetTrigger("SlideDown");
        }
        else
        {
            buildingMenuAnimator.SetTrigger("SlideUp");
        }

        
        isBuildingMenuUp = !isBuildingMenuUp;
    }

    public void UpdateBuildingMenuEntries()
    {
        BuildingListSelectButtonScript[] entries = buildingViewPortContent.GetComponentsInChildren<BuildingListSelectButtonScript>();
        foreach (BuildingListSelectButtonScript entry in entries)
        {
            int goldInPossession;
            if (player.synchronizedPlayerGameData.teamNumber == 1) goldInPossession = LocalStateManager.instance.localGameData.goldTeam1;
            else goldInPossession = LocalStateManager.instance.localGameData.goldTeam2;
            entry.UpdateGoldInteractable(goldInPossession);
        }
    }

    public void TurnTownManagerUI(bool turnOn)
    {
        if (turnOn) townManagerUI.SetActive(true);
        else townManagerUI.SetActive(false);
    }

    public void TurnMinerUI(bool turnOn)
    {
        if (turnOn) minerUI.SetActive(true) ;
        else minerUI.SetActive(false) ;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInBuildingMode && currentBuildingObject)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = mainCamera.ScreenToWorldPoint(mousePos);
            Vector3 buildingPos = new Vector3(mousePos.x, mousePos.y, -3);
            currentBuildingObject.gameObject.transform.position = buildingPos;
            Building buildingComponent = currentBuildingObject.GetComponent<Building>();

            if (Input.GetMouseButtonDown(0) && !buildingComponent.IsColliding() && !buildingComponent.IsOutOfBounds() && !buildingComponent.IsFarFromPlayer())
            {
                PlayerNetworking currentPlayer = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
                GameManager.instance.buildingManager.PlaceBuilding(currentBuildingData.buildingName, buildingPos, currentPlayer.synchronizedPlayerGameData.matchPtr.matchID, currentPlayer); ;
                Destroy(currentBuildingObject);
                isInBuildingMode = false;
                currentBuildingData.currentlyBuiltAmount += 1;

                string matchID = player.synchronizedPlayerGameData.matchPtr.matchID;
                int teamNum = player.synchronizedPlayerGameData.teamNumber;
                ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, -currentBuildingData.costGold, Resource.GOLD);
                GameManager.instance.CMDUpdateResource(msg);
            }
        }

        // Update resources text
        GameData gameData = LocalStateManager.instance.localGameData;
        if (player.synchronizedPlayerGameData.role == GameRole.TOWN_MANAGER)
        {
            int playerTeam = player.synchronizedPlayerGameData.teamNumber;
            goldAmountText.text = gameData.GetGold(playerTeam).ToString();
            foodAmountText.text = gameData.GetFood(playerTeam).ToString();
            mineralsAmountText.text = gameData.GetMinerals(playerTeam).ToString();
        }
        else if (player.synchronizedPlayerGameData.role == GameRole.MINER)
        {
            int playerTeam = player.synchronizedPlayerGameData.teamNumber;
            goldOreAmountText.text = gameData.GetGoldOre(playerTeam).ToString();
            mineralOreAmountText.text = gameData.GetMinerals(playerTeam).ToString(); 
        }
    }
}
