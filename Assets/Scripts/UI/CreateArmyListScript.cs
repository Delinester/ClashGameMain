using System.Collections;
using System.Collections.Generic;
using System.Data;
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

    [SerializeField]
    private GameObject worldMapArmyPrefab;

    private GameUI gameUI;
    private Vector2 clickPosition;
    private PlayerNetworking player;
    private ArmyCreateListEntry[] entries;

    public void ResetTroopsCount()
    {
        foreach (ArmyCreateListEntry entry in content.GetComponentsInChildren<ArmyCreateListEntry>())
        {
            entry.ResetTroopsCount();
        }
    }

    public void OnCreateArmyButtonPressed()
    {
        Vector3 playerPosition = LocalStateManager.instance.localPlayerCharacter.transform.position;
        GameObject armyObject = Instantiate(worldMapArmyPrefab, playerPosition, worldMapArmyPrefab.transform.rotation);
        string hash = LobbyManager.instance.GenerateRandomString(20);
        armyObject.GetComponent<CharacterControllerBase>().SetHash(hash);
        GameManager.instance.CMDSpawnPuppetOnClients(player.synchronizedPlayerGameData.matchPtr.matchID, player.GetUserData().username, hash, PuppetType.ARMY, playerPosition);
        
        armyObject.GetComponent<WorldMapArmyAI>().MoveToPoint(clickPosition);
        Troop[] troopsList = new Troop[3];
        if (entries == null) Debug.LogError("Entries is null");
        for (int i = 0; i < entries.Length; i++)
        {
            troopsList[i] = new Troop();
            troopsList[i].data = entries[i].GetTroopData();
            troopsList[i].count = entries[i].GetTroopCount();
        }
        armyObject.GetComponent<WorldMapArmyAI>().SetTroopsInArmy(troopsList);
    }
    public void SetClickPosition(Vector2 pos)
    {
        clickPosition = pos;
    }
    void Awake()
    {
        gameUI = FindObjectOfType<GameUI>();
        closeButton.onClick.AddListener(() => gameUI.DisableArmyCreationMenu());
        createArmyButton.onClick.AddListener(() => OnCreateArmyButtonPressed());

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
        player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        entries = content.GetComponentsInChildren<ArmyCreateListEntry>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
