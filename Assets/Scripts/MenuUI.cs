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
    public void ConnectToServer()
    {
        NetworkManager manager = NetworkManager.singleton;
        if (!NetworkClient.isConnected)
        {
            manager.StartClient();
        }

        if (NetworkClient.isConnected)
        {
            statusText.text = "Connected to " + manager.networkAddress;
            connectButton.enabled = false;

        }
        else statusText.text = "Connection failed to " + manager.networkAddress;
    }

    public void CreateServer()
    {
        NetworkManager.singleton.StartServer();
        if (NetworkServer.active)
            statusText.text = "Server created";
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
