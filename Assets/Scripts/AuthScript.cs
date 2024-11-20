using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;
using System.Text;

public class AuthScript : NetworkBehaviour
{
    private const string baseAPIurl = "https://os-project-ten.vercel.app";
    private const string getUserEndpoint = "/users/get";
    private const string registerUserEndpoint = "/users/create";

    private delegate void ResponseHandler(NetworkConnection conn, string response);
    private delegate void PostResponseHandler(NetworkConnectionToClient conn, int code);


    private PlayerNetworking playerNetworking;
    private MenuUI menuController;

    public void Login(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {
        //Debug.Log("Client is " + conn.address);
        LoadingCanvas.instance.gameObject.SetActive(true);
        playerNetworking = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        playerNetworking.AssignUserData(userData);
        menuController = FindObjectOfType<MenuUI>();
        LoginUser_Server(conn);
    }

    [Command]
    private void LoginUser_Server(NetworkConnectionToClient conn)
    {
        StartCoroutine(GetRequest(baseAPIurl, getUserEndpoint, LoginUser_Client, conn));        
    }

    [TargetRpc]
    private void LoginUser_Client(NetworkConnection conn, string response)
    {        
        PlayerNetworking.UserData userData = playerNetworking.GetUserData();
        LoadingCanvas.instance.gameObject.SetActive(false);
        // Parse JSON response
        try
        {
            //This assumes your API returns a JSON array. Adjust accordingly if it's a single object.
            PlayerNetworking.UserData[] userDataArray = JsonHelper.FromJson<PlayerNetworking.UserData>(JsonHelper.fixJson(response));

            if (userDataArray.Length > 0)
            {
                
                foreach (PlayerNetworking.UserData data in userDataArray)
                {
                    if (data.username == userData.username)
                    {
                        menuController.SetStatusString("Welcome back, " + data.username);
                        Debug.Log("Username: " + data.username);
                        menuController.UnlockPlayButton();
                        return;
                    }
                }
                menuController.SetStatusString("User " + userData.username + " is not found!");
            }
            else
            {
                menuController.SetStatusString("Error with JSON response!");
                Debug.LogError("No data found in JSON response.");
            }
        }

        catch (System.Exception e)
        {
            Debug.LogError("Error parsing JSON: " + e.Message);
            Debug.LogError("JSON Response: " + response);
        }
    }

    public void Register(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {
        LoadingCanvas.instance.gameObject.SetActive(true);
        playerNetworking = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        //Debug.Log("Client is " + conn.address);
        menuController = FindObjectOfType<MenuUI>();
        Debug.Log("Register userData is " + userData.GetType());
        RegisterUser_Server(userData, conn);
        Debug.Log("Register initiated");
    }

    [Command]
    private void RegisterUser_Server(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {

        //Debug.LogError("RegisterUser_Server userData is " + userData.GetType());
        StartCoroutine(PostRequest(baseAPIurl, registerUserEndpoint, RegisterUser_Client, userData, conn));
    }

    [TargetRpc]
    private void RegisterUser_Client(NetworkConnectionToClient conn, int code)
    {
        LoadingCanvas.instance.gameObject.SetActive(false);
        Debug.Log("Client RPC reached");
        if (code == 201)
        {
            menuController.SetStatusString("Registered successfully");
        }
        else
        {
            menuController.SetStatusString("User cannot be registered");
        }
    }
    private IEnumerator GetRequest(string uri, string endpoint, ResponseHandler handler, NetworkConnection conn = null)
    {
        
        //Debug.LogError("Type is " + conn.GetType());
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + endpoint))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                
            }
            else
            {
                handler(conn, webRequest.downloadHandler.text);
            }
        }
        
    }

    private IEnumerator PostRequest(string uri, string endpoint, PostResponseHandler handler, PlayerNetworking.UserData postData, NetworkConnectionToClient conn)
    {
        
        string json = JsonUtility.ToJson(postData);
        //Debug.LogError("Type is " + conn.GetType() + " Another is " + postData.GetType());
        using (UnityWebRequest www = new UnityWebRequest(uri + endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                if (www.responseCode == 201)
                {
                    //Debug.LogError("POST Request Successful!");
                }
                else
                {
                    Debug.LogError("Error in POST response!");
                }
                Debug.Log("Response: " + responseText);       
            }
            handler(conn, (int)www.responseCode);
        }
        
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }
        public static string fixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }
        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
