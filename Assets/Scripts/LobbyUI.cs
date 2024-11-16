using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [SerializeField]
    private GameObject listContentObject;

    [SerializeField]
    private GameObject listEntryPrefab;
    // Start is called before the first frame update

    void Start()
    {
       //playerNameText.text = "You are "+ LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().GetUserData().username;

       for (int i = 0; i < 25; i++) 
        {
            GameObject obj = Instantiate(listEntryPrefab, listContentObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
