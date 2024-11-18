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
            gameData.teamNumber = 1;
            Debug.Log("Before assigning matchID is " + player.GetGameData().matchPtr.matchID);
            player.AssignGameData(gameData);
            Debug.Log("After assigning matchID is " + player.GetGameData().matchPtr.matchID);
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
            gameData.teamNumber = 2;
            player.AssignGameData(gameData);
            LobbyManager.instance.CMDSendWaitingRoomUpdateRPC(player.synchronizedPlayerGameData.matchPtr);
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
            Transform contentTransform = p.synchronizedPlayerGameData.teamNumber == 1 ? playerList1ContentObject.transform : playerList2ContentObject.transform;
            GameObject playerObject = Instantiate(playerListEntryPrefab, contentTransform);
            playerObject.GetComponent<PlayerListEntry>().playerNameText.text = p.GetUserData().username;
        }
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
