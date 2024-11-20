using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [Header("List view panel")]
    [SerializeField]
    private GameObject listViewObject;
    [SerializeField]
    private GameObject listContentObject;
    [SerializeField]
    private GameObject listEntryPrefab;

    [Header("Room creation")]
    [SerializeField]
    private GameObject createRoomPanel;
    [SerializeField]
    private TMP_InputField nameInputField;

    [Header("Waiting Room")]
    [SerializeField]
    private GameObject waitingRoomObject;
    [SerializeField]
    private GameObject playerListEntryPrefab;
    [SerializeField]
    private GameObject playerList1ContentObject;
    [SerializeField]
    private GameObject playerList2ContentObject;
    [SerializeField]
    private Sprite warriorIcon;
    [SerializeField]
    private Sprite townmanagerIcon;
    [SerializeField]
    private Sprite minerIcon;

    // Start is called before the first frame update
    public void OpenCreateRoomPanel()
    {
        listViewObject.SetActive(false);
        waitingRoomObject.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void OpenRoomsList()
    {
        listViewObject.SetActive(true);
        waitingRoomObject.SetActive(false);
        createRoomPanel.SetActive(false);
    }
    public void OpenWaitingRoom()
    {
        listViewObject.SetActive(false);
        waitingRoomObject.SetActive(true);
        createRoomPanel.SetActive(false);
    }

    public void OnCreateRoomPressed()
    {
        OpenCreateRoomPanel();
    }

    public void OnChangeTeamLeftPressed()
    {
        PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        PlayerGameData gameData = player.GetGameData();
        Debug.Log("Change team left");
        if (gameData.teamNumber != 1)
        {
            Debug.Log("MUST CHANGE TEAM TO 1");
            PlayerGameData playerGameData = new PlayerGameData(gameData);
            playerGameData.teamNumber = 1;

            //Debug.Log("Before assigning matchID is " + player.GetGameData().matchPtr.matchID);
            player.CMDAssignGameData(playerGameData);
           // Debug.Log("After assigning matchID is " + player.GetGameData().matchPtr.matchID);
            //UpdateWaitingRoom(gameData.matchPtr);
            LobbyManager.instance.CMDSendWaitingRoomUpdateRPC(player.synchronizedPlayerGameData.matchPtr);
        }
    }

    public void OnChangeTeamRightPressed()
    {
        PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        PlayerGameData gameData = player.GetGameData();
        Debug.Log("Change team right");
        if (gameData.teamNumber != 2)
        {
            Debug.Log("MUST CHANGE TEAM TO 2");
            PlayerGameData playerGameData = new PlayerGameData(gameData);
            playerGameData.teamNumber = 2;
            player.CMDAssignGameData(playerGameData);

            Debug.Log("Team number of player in PLAYER DATA is " + player.synchronizedPlayerGameData.teamNumber);
            LobbyManager.instance.CMDSendWaitingRoomUpdateRPC(player.synchronizedPlayerGameData.matchPtr);

            Debug.Log("Team number of player AFTER RPC ROOM UPDATE IS is " + player.synchronizedPlayerGameData.teamNumber);
        }
    }

    public void UpdateWaitingRoom(Match matchInfo)
    {
        PlayerListEntry[] playersObjects = playerList1ContentObject.GetComponentsInChildren<PlayerListEntry>();
        foreach (PlayerListEntry playerListEntry in playersObjects)
        {
            Destroy(playerListEntry.gameObject);
        }
        PlayerListEntry[] playersObjects2 = playerList2ContentObject.GetComponentsInChildren<PlayerListEntry>();
        foreach (PlayerListEntry playerListEntry in playersObjects2)
        {
            Destroy(playerListEntry.gameObject);
        }

        foreach (PlayerNetworking p in matchInfo.players)
        {
            Debug.Log("Player is in team number " + p.synchronizedPlayerGameData.teamNumber);
          //  Debug.Log("PlayerRRR is in team number " + LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber);
            Transform contentTransform = p.synchronizedPlayerGameData.teamNumber == 1 ? playerList1ContentObject.transform : playerList2ContentObject.transform;
            GameObject playerObject = Instantiate(playerListEntryPrefab, contentTransform);

            PlayerListEntry playerListEntry = playerObject.GetComponent<PlayerListEntry>();
            playerListEntry.playerNameText.text = p.GetUserData().username;
            switch (p.GetGameData().role)
            {
                case GameRole.WARRIOR: playerListEntry.roleImage.sprite = warriorIcon; break;
                case GameRole.TOWN_MANAGER: playerListEntry.roleImage.sprite = townmanagerIcon; break;
                case GameRole.MINER: playerListEntry.roleImage.sprite = minerIcon; break;
            }
        }
    }

    public void OnChangeRolePressed(GameObject button)
    {
        PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        PlayerGameData playerGameData = new PlayerGameData(player.synchronizedPlayerGameData);
        playerGameData.role = button.GetComponent<RoleChangeButton>().buttonRole;
        player.CMDAssignGameData(playerGameData);

        LobbyManager.instance.CMDSendWaitingRoomUpdateRPC(player.synchronizedPlayerGameData.matchPtr);
    }

    public void UpdateRoomsList()
    {
        // TODO!!!
    }

    public void OnConfirmCreateRoomPressed()
    {
        string matchName = nameInputField.text;
        Debug.Log("Match is " + matchName);
        LobbyManager.instance.CreateMatch(matchName, LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>());
    }

    public void OnJoinRoomPressed(RoomListEntry entry)
    {
        Debug.Log("Join Button Pressed");
        Match match = entry.matchPtr;
        LobbyManager.instance.CMDJoinMatch(match.matchID, LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>());
    }

    void Start()
    {
       //playerNameText.text = "You are "+ LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().GetUserData().username;
       LobbyManager manager = LobbyManager.instance;
       for (int i = 0; i < manager.matchesList.Count; i++)
        {
            GameObject obj = Instantiate(listEntryPrefab, listContentObject.transform);
            RoomListEntry listEntry = obj.GetComponent<RoomListEntry>();
            listEntry.matchPtr = manager.matchesList[i];
            listEntry.numberOfPlayers.text = manager.matchesList[i].players.Count.ToString() + "/4";
            listEntry.nameText.text = manager.matchesList[i].matchName;

            //obj.GetComponentInChildren<JoinRoomButtonScript>().gameObject.GetComponent<Button>().clicked += () => OnJoinRoomPressed(listEntry);
        }
       //for (int i = 0; i < 25; i++) 
       // {
       //     GameObject obj = Instantiate(listEntryPrefab, listContentObject.transform);
       // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
