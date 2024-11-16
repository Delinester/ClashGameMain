using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    // Start is called before the first frame update

    public void OnCreateRoomPressed()
    {
        listViewObject.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void OnConfirmCreateRoomPressed()
    {
        string matchName = nameInputField.text;
        Debug.Log("Match is " + matchName);
        LobbyManager.instance.CreateMatch(matchName, LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>());
    }
    void Start()
    {

       //playerNameText.text = "You are "+ LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().GetUserData().username;
       LobbyManager manager = LobbyManager.instance;
       for (int i = 0; i < manager.matchesList.Count; i++)
        {
            GameObject obj = Instantiate(listEntryPrefab, listContentObject.transform);
            obj.GetComponent<RoomListEntry>().nameText.text = manager.matchesList[i].matchName;
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
