using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Building Menu")]
    [SerializeField]
    private GameObject buildingViewPortContent;

    [SerializeField]
    private GameObject buildingListEntryPrefab;

    ////////////////////////////////////////////////////////
    private bool isInBuildingMode = false;
    private GameObject currentBuildingObject;
    private BuildingData currentBuildingData;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
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
                GameManager.instance.buildingManager.PlaceBuilding(currentBuildingData.buildingName, buildingPos);
                Destroy(currentBuildingObject);
                isInBuildingMode = false;
            }
        }
    }
}
