using UnityEngine;
using TMPro;

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
    [SerializeField]
    private TextMeshProUGUI damageBuffText;
    //////////////////////////////////////////////
    [Header("Warrior tab")]
    [SerializeField]
    private TextMeshProUGUI barbariansCountText;
    [SerializeField]
    private TextMeshProUGUI mercenariesCountText;
    [SerializeField]
    private TextMeshProUGUI heavyKnightsCountText;
    [SerializeField]
    private TextMeshProUGUI warriorGoldResourceText;
    [SerializeField]
    private TextMeshProUGUI warriorFoodResourceText;
    [SerializeField]
    private GameObject createArmyMenuObject;

    private bool isInBuildingMode = false;
    private bool isArmyChosen = false;

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
    private GameObject warriorWorldMapUI;

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

        createArmyMenuObject.GetComponentInChildren<Canvas>().enabled = false;
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
            goldInPossession = LocalStateManager.instance.localGameData.GetGold(player.synchronizedPlayerGameData.teamNumber);
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

    public void TurnWarriorUI(bool turnOn)
    {
        if (turnOn) warriorWorldMapUI.SetActive(true);
        else warriorWorldMapUI.SetActive(false);
    }

    public void DisplayArmyCreationMenu(Vector3 position)
    {
        createArmyMenuObject.transform.position = position;
        createArmyMenuObject.GetComponentInChildren<Canvas>().enabled = true;
        createArmyMenuObject.GetComponent<CreateArmyListScript>().SetClickPosition(position);
    }

    public void DisableArmyCreationMenu()
    {
        createArmyMenuObject.GetComponentInChildren<Canvas>().enabled = false;
        createArmyMenuObject.GetComponent<CreateArmyListScript>().ResetTroopsCount();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = mainCamera.ScreenToWorldPoint(mousePos);
        if (player.synchronizedPlayerGameData.role == GameRole.TOWN_MANAGER && isInBuildingMode && currentBuildingObject)
        {
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
        int playerTeam = player.synchronizedPlayerGameData.teamNumber;
        if (player.synchronizedPlayerGameData.role == GameRole.TOWN_MANAGER)
        {
            goldAmountText.text = gameData.GetGold(playerTeam).ToString();
            foodAmountText.text = gameData.GetFood(playerTeam).ToString();
            mineralsAmountText.text = gameData.GetMinerals(playerTeam).ToString();
        }
        else if (player.synchronizedPlayerGameData.role == GameRole.MINER)
        {           
            goldOreAmountText.text = gameData.GetGoldOre(playerTeam).ToString();
            mineralOreAmountText.text = gameData.GetMinerals(playerTeam).ToString(); 
            damageBuffText.text = ((MinerController)playerController).GetDamageBuffPercent().ToString() + "%";
        }
        else if (player.synchronizedPlayerGameData.role == GameRole.WARRIOR)
        {
            barbariansCountText.text = gameData.GetBarbarians(playerTeam).ToString();
            mercenariesCountText.text = gameData.GetMercenaries(playerTeam).ToString();
            heavyKnightsCountText.text = gameData.GetHeavyKnights(playerTeam).ToString();
            warriorGoldResourceText.text = gameData.GetGold(playerTeam).ToString();
            warriorFoodResourceText.text = gameData.GetFood(playerTeam).ToString();
        }
    }
}
