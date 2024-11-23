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
    private BuildingData buildingData;

    public void SetBuildingData(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        buildingNameText.text = buildingData.buildingName;
        buildingImage.sprite = buildingData.buildingSprite;
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
