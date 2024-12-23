using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateArmyListScript : MonoBehaviour
{
    [SerializeField]
    TroopData[] troops;
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private GameObject entryPrefab;

    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button createArmyButton;

    private GameUI gameUI;

    public void ResetTroopsCount()
    {
        foreach (ArmyCreateListEntry entry in content.GetComponentsInChildren<ArmyCreateListEntry>())
        {
            entry.ResetTroopsCount();
        }
    }
    void Awake()
    {
        gameUI = FindObjectOfType<GameUI>();
        closeButton.onClick.AddListener(() => gameUI.DisableArmyCreationMenu());

        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (TroopData data in troops)
        {
            GameObject obj = Instantiate(entryPrefab, content.transform);
            obj.GetComponent<ArmyCreateListEntry>().InitTroop(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
