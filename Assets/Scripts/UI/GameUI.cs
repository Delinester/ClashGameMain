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

    [SerializeField]
    private BuildingData[] buildingsDataArray;

    // Start is called before the first frame update
    void Start()
    {
        foreach (BuildingData building in buildingsDataArray)
        {
            GameObject obj = Instantiate(buildingListEntryPrefab, buildingViewPortContent.transform);
            obj.GetComponent<BuildingListEntryScript>().SetBuildingData(building);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
