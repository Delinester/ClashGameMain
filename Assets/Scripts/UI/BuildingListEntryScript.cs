using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingListEntryScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buildingNameText;
    [SerializeField]
    private Image buildingImage;
    [SerializeField]
    private TextMeshProUGUI costGoldText;
    [SerializeField]
    private BuildingData buildingData;

    public BuildingData GetBuildingData()
    {
        return buildingData;
    }
    public void SetBuildingData(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        buildingNameText.text = buildingData.buildingName;
        buildingImage.sprite = buildingData.buildingSprite;
        costGoldText.text = buildingData.costGold.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
