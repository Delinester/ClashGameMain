using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingListSelectButtonScript : MonoBehaviour
{
    public BuildingData buildingData;
    public GameUI gameUI;

    private bool isDisabled = false;
    void Awake()
    {
        gameUI = FindObjectOfType<GameUI>();
    }
    // Start is called before the first frame update
    void Start()
    {
        buildingData = GetComponentInParent<BuildingListEntryScript>().GetBuildingData();
        GetComponent<Button>().onClick.AddListener(delegate () { gameUI.EnterBuidingMode(buildingData); });
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
