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
    private int teamNumber;
    private string matchID;
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
        Troop[] troopsList = new Troop[3];
        if (entries == null) Debug.LogError("Entries is null");

        bool isFullZero = true;
        for (int i = 0; i < entries.Length; i++)
        {
            troopsList[i] = new Troop();
            TroopData data = entries[i].GetTroopData();
            int count = entries[i].GetTroopCount();
            troopsList[i].data = data;
            troopsList[i].count = count;

            if (count != 0) isFullZero = false;
            if (data.troopType == Resource.BARBARIAN && LocalStateManager.instance.localGameData.GetBarbarians(teamNumber) >= count)
            {
                ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNumber, -count, Resource.BARBARIAN);
                GameManager.instance.CMDUpdateResource(msg);
            }
            else if (data.troopType == Resource.MERCENARY && LocalStateManager.instance.localGameData.GetMercenaries(teamNumber) >= count)
            {
                ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNumber, -count, Resource.MERCENARY);
                GameManager.instance.CMDUpdateResource(msg);
            }
            else if (data.troopType == Resource.HEAVY_KNIGHT && LocalStateManager.instance.localGameData.GetHeavyKnights(teamNumber) >= count)
            {
                ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNumber, -count, Resource.HEAVY_KNIGHT);
                GameManager.instance.CMDUpdateResource(msg);
            }
            else return;
        }
        if (isFullZero) return;

        Vector3 playerPosition = LocalStateManager.instance.localPlayerCharacter.transform.position;
        GameObject armyObject = Instantiate(worldMapArmyPrefab, playerPosition, worldMapArmyPrefab.transform.rotation);
        string hash = LobbyManager.instance.GenerateRandomString(20);
        armyObject.GetComponent<CharacterControllerBase>().SetHash(hash);
        //GameManager.instance.CMDSpawnPuppetOnClients(player.synchronizedPlayerGameData.matchPtr.matchID, player.GetUserData().username, hash, PuppetType.ARMY, playerPosition);
        Army army = new Army(troopsList);
        army.ownerTeamNum = teamNumber;
        GameManager.instance.CMDSpawnArmyPuppetOnClients(player.synchronizedPlayerGameData.matchPtr.matchID, player.GetUserData().username, hash, army, playerPosition);
        armyObject.GetComponent<WorldMapArmyAI>().MoveToPoint(clickPosition);        
        armyObject.GetComponent<WorldMapArmyAI>().SetTroopsInArmy(troopsList);
        armyObject.GetComponent<WorldMapArmyAI>().SetArmyOwnerTeam(teamNumber);
        armyObject.GetComponent<WorldMapArmyAI>().SetBaseOutline();
        GameManager.instance.battlesManager.AddNewArmy(armyObject.GetComponent<WorldMapArmyAI>());
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
        teamNumber = player.synchronizedPlayerGameData.teamNumber;
        matchID = player.synchronizedPlayerGameData.matchPtr.matchID;
        entries = content.GetComponentsInChildren<ArmyCreateListEntry>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
