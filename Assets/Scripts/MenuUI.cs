using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public Button connectButton;
    public TMP_InputField usernameInputField;

    private PlayerNetworking playerNetworking;
    private AuthScript authScript;
    private NetworkConnectionToClient clientConn;

    [SerializeField]
    private GameObject allowWhenClientConnected;
    [SerializeField]
    private GameObject playButtonObject;
    [SerializeField]
    private GameObject connectButtonObject;

    public void ConnectToServer()
    {
        NetworkManager manager = NetworkManager.singleton;
        if (!NetworkClient.isConnected)
        {
            manager.StartClient();
            StartCoroutine(DelayedLoginTextCheck());
        }        
    }

    private IEnumerator DelayedLoginTextCheck()
    {
        yield return new WaitForSeconds(0.5f);
        if (NetworkClient.isConnected)
        {
            statusText.text = "Connected to " + NetworkManager.singleton.networkAddress;
            connectButton.enabled = false;
            UnlockClientConnectMenu();
        }
        else statusText.text = "Connection failed to " + NetworkManager.singleton.networkAddress;
    }

    public void CreateServer()
    {
        NetworkManager.singleton.StartServer();
        if (NetworkServer.active)
        {
            statusText.text = "Server created";
            connectButtonObject.SetActive(false);
        }
        else statusText.text = "Server cannot be created";
    }

    public void OnLoginButtonPressed()
    {
        authScript = FindObjectOfType<AuthScript>();
        playerNetworking = FindObjectOfType<PlayerNetworking>();
        clientConn = authScript.transform.gameObject.GetComponent<NetworkIdentity>().connectionToClient;

        if (!CheckInputFields()) { return; }
        PlayerNetworking.UserData userData = new PlayerNetworking.UserData();
        userData.username = usernameInputField.text;
        authScript.Login(userData, clientConn);
    }

    public void OnRegisterButtonPressed()
    {
        authScript = FindObjectOfType<AuthScript>();
        playerNetworking = FindObjectOfType<PlayerNetworking>();
        clientConn = authScript.transform.gameObject.GetComponent<NetworkIdentity>().connectionToClient;

        if (!CheckInputFields()) { return; }

        PlayerNetworking.UserData userData = new PlayerNetworking.UserData();
        userData.username = usernameInputField.text;
        Debug.Log("MenuUI userData is " + userData.GetType());
        authScript.Register(userData, clientConn);
    }

    public void OnPlayButtonPressed()
    {
        playerNetworking.ChangeScene("Lobby");
    }

    public void UnlockClientConnectMenu()
    {
        allowWhenClientConnected.SetActive(true);
    }

    public void UnlockPlayButton()
    {
        playButtonObject.SetActive(true);
    }

    private bool CheckInputFields()
    {
        if (usernameInputField.text.Length < 5 || usernameInputField.text.Length > 12)
        {
            statusText.text = "Wrong username format(5-12 chars)";
            return false;
        }
        return true;
    }



    public void SetStatusString(string text)
    {
        statusText.text = text;
    }

    void Awake()
    {
        playButtonObject.SetActive(false);
        allowWhenClientConnected.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
