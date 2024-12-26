using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;
using kcp2k;

public class MenuUI : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public Button connectButton;
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    [Header("Registration")]
    public TMP_InputField usernameRegField;
    public TMP_InputField nameRegField;
    public TMP_InputField surnameRegField;
    public TMP_Dropdown genderDropdown;
    public TMP_InputField bdayRegField;
    public TMP_InputField ageRegField;
    public TMP_InputField emailRegField;
    public TMP_InputField addressRegField;
    public TMP_InputField passwordRegField;
    public TMP_InputField iconidRegField;

    public TextMeshProUGUI regStatusString;

    private PlayerNetworking playerNetworking;
    private APIConnector apiConnector;
    private NetworkConnectionToClient clientConn;

    [SerializeField]
    private GameObject allowWhenClientConnected;
    [SerializeField]
    private GameObject playButtonObject;
    [SerializeField]
    private GameObject connectButtonObject;
    [SerializeField]
    private GameObject registrationMenu;

    [SerializeField]
    private TMP_InputField ipField;
    [SerializeField]
    private TMP_InputField portField;

    private string ip;
    private int port;

    public void ConnectToServer()
    {
        string ip = ipField.text;
        if (ip != string.Empty)
        {
            NetworkManager.singleton.networkAddress = ipField.text;
            NetworkManager.singleton.GetComponent<KcpTransport>().port = Convert.ToUInt16(portField.text);
        }
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
        string ip = ipField.text;
        if (ip != string.Empty)
        {
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.GetComponent<KcpTransport>().port = Convert.ToUInt16(portField.text);
        }
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
        apiConnector = FindObjectOfType<APIConnector>();
        playerNetworking = FindObjectOfType<PlayerNetworking>();
        clientConn = apiConnector.transform.gameObject.GetComponent<NetworkIdentity>().connectionToClient;

        if (!CheckInputFields()) { return; }
        PlayerNetworking.UserData userData = new PlayerNetworking.UserData();
        userData.username = usernameInputField.text;
        userData.password = passwordInputField.text;
        apiConnector.Login(userData, clientConn);
    }

    public void OpenRegistrationMenu()
    {

        registrationMenu.SetActive(true);
    }

    public void CloseRegistrationMenu()
    {
        registrationMenu.SetActive(false);
    }

    public void OnRegisterButtonPressed()
    {
        apiConnector = FindObjectOfType<APIConnector>();
        playerNetworking = FindObjectOfType<PlayerNetworking>();
        clientConn = apiConnector.transform.gameObject.GetComponent<NetworkIdentity>().connectionToClient;

        //if (!CheckInputFields()) { return; }

        PlayerNetworking.UserData userData = new PlayerNetworking.UserData();
        userData.username = usernameRegField.text;
        userData.name = nameRegField.text;
        userData.surname = surnameRegField.text;
        userData.gender = genderDropdown.itemText.text;
        userData.b_date = bdayRegField.text;
        userData.age = int.Parse(ageRegField.text);
        userData.email = emailRegField.text;
        userData.address = addressRegField.text;
        userData.password = passwordRegField.text;
        userData.icon_id = int.Parse(iconidRegField.text);
        
        //Debug.Log("MenuUI userData is " + userData.GetType());
        apiConnector.Register(userData, clientConn);
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

    public void SetRegStatusString(string text)
    {
        regStatusString.text = text;
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
