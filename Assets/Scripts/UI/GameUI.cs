using System.Collections;
using System.Collections.Generic;
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
    private bool isInBuildingMode = false;
    private GameObject currentBuildingObject;
    private BuildingData currentBuildingData;
    private Camera mainCamera;
    private PlayerNetworking player;

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        goldAmountText.text = "0";
        foodAmountText.text = "0";
        mineralsAmountText.text = "0";
        player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (BuildingData building in GameManager.instance.buildingManager.GetBuildingsDataList())
        {
            GameObject obj = Instantiate(buildingListEntryPrefab, buildingViewPortContent.transform);
            obj.GetComponent<BuildingListEntryScript>().SetBuildingData(building);
        }
    }

    public void EnterBuidingMode(BuildingData building)
    {
        currentBuildingData = building;
        isInBuildingMode = true;
        currentBuildingObject = Instantiate(building.buildingPrefab);
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

            if (Input.GetMouseButtonDown(0))
            {
                PlayerNetworking currentPlayer = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
                GameManager.instance.buildingManager.PlaceBuilding(currentBuildingData.buildingName, buildingPos, currentPlayer.synchronizedPlayerGameData.matchPtr.matchID, currentPlayer); ;
                Destroy(currentBuildingObject);
                isInBuildingMode = false;
                currentBuildingData.currentlyBuiltAmount += 1;
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
    }
}
