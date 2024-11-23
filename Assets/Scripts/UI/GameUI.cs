using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Building Menu")]
    [SerializeField]
    private GameObject viewPortContent;

    [SerializeField]
    private GameObject buildingListEntryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(buildingListEntryPrefab, viewPortContent.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
