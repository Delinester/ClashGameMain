using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopsTrainingListScript : MonoBehaviour
{
    [SerializeField]
    private GameObject contentObject;
    [SerializeField] 
    GameObject troopListEntryPrefab;
    [SerializeField]
    private TroopData[] troopsDataArray;
    
    private PlayerTown playerTown;

    [SerializeField]
    private Canvas localUIcanvas;

    // Start is called before the first frame update
    void Start()
    {
        localUIcanvas.worldCamera = Camera.main;
        playerTown = GetComponent<PlayerTown>();
        foreach (TroopData data in troopsDataArray)
        {
            GameObject obj = Instantiate(troopListEntryPrefab, contentObject.transform);
            obj.GetComponent<TroopTrainingListEntry>().InitTroop(data, playerTown);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
