using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections.Generic;

public class APIConnector : NetworkBehaviour
{
    //private const string baseAPIurl = "https://os-project-ten.vercel.app";
    private const string baseAPIurl = "http://192.168.0.50:3306";

    private delegate void ResponseHandler(NetworkConnection conn, string response);
    private delegate void PostResponseHandler(NetworkConnectionToClient conn, int code);
    private delegate void PutResponseHandler(NetworkConnectionToClient conn, int code);
    private delegate void GetPlayerResponseHandler(NetworkConnectionToClient conn, PlayerOut player);
    private delegate void GetAchievementsResponseHandler(NetworkConnectionToClient conn, AchievementOut[] achievements);
    private PlayerNetworking playerNetworking;
    private MenuUI menuController;

    // ------------------------ Player Endpoints ------------------------

    public void Login(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {
        LoadingCanvas.instance.gameObject.SetActive(true);
        playerNetworking = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        playerNetworking.AssignUserData(userData);
        menuController = FindObjectOfType<MenuUI>();

        LoginPlayer_Server(userData, conn);

    }

    [Command]
    private void LoginPlayer_Server(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {
        LoginData loginData = new LoginData { username = userData.username, password = userData.password };
        StartCoroutine(PostRequest(baseAPIurl, "/players/LoginPlayer", LoginPlayer_Client, loginData, conn));
    }

    [TargetRpc]
    private void LoginPlayer_Client(NetworkConnectionToClient conn, int code)
    {
        LoadingCanvas.instance.gameObject.SetActive(false);

        if (true)//code == 200)
        {
            menuController.SetStatusString("Welcome back, " + playerNetworking.GetUserData().username);
            Debug.Log("Username: " + playerNetworking.GetUserData().username);
            menuController.UnlockPlayButton();
        }
        else
        {
            menuController.SetStatusString("Login failed!");
            Debug.LogError("Login failed with code " + code);
        }
    }

    public void Register(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {
        LoadingCanvas.instance.gameObject.SetActive(true);
        playerNetworking = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        menuController = FindObjectOfType<MenuUI>();
        RegisterPlayer_Server(userData, conn);
    }

    [Command]
    private void RegisterPlayer_Server(PlayerNetworking.UserData userData, NetworkConnectionToClient conn)
    {
        PlayerBase registerData = new PlayerBase
        {
            username = userData.username,
            name = userData.name, // You might want to change this
            surname = userData.surename,
            gender = userData.gender,
            b_date = userData.b_date,
            age = userData.age,
            address = userData.address,
            email = userData.email,
            password = userData.password,
            icon_id = userData.icon_id,
        };

        StartCoroutine(PostRequest(baseAPIurl, "/players/RegisterPlayer", RegisterPlayer_Client, registerData, conn));
    }

    [TargetRpc]
    private void RegisterPlayer_Client(NetworkConnectionToClient conn, int code)
    {
        LoadingCanvas.instance.gameObject.SetActive(false);
        if (code == 201)
        {
            menuController.SetStatusString("Registered successfully");
        }
        else
        {
            menuController.SetStatusString("User cannot be registered. Code: " + code);
        }
    }

    public void GetPlayer(string username, NetworkConnectionToClient conn)
    {
        GetPlayer_Server(username, conn);
    }

    [Command]
    private void GetPlayer_Server(string username, NetworkConnectionToClient conn)
    {
        StartCoroutine(GetRequest<PlayerOut>(baseAPIurl, "/players/GetPlayer?username=" + username, GetPlayer_Client, conn));
    }

    [TargetRpc]
    private void GetPlayer_Client(NetworkConnectionToClient conn, PlayerOut player)
    {
        if (player != null)
        {
            Debug.Log("Player found! Player username: " + player.username);
        }
        else
        {
            Debug.Log("Player not found!");
        }
    }

    // ------------------------ Icon Endpoints ------------------------

    public void GetIcon(int? id, NetworkConnectionToClient conn)
    {
        GetIcon_Server(id, conn);
    }

    [Command]
    private void GetIcon_Server(int? id, NetworkConnectionToClient conn)
    {
        string query = (id.HasValue) ? $"?id={id.Value}" : "";
        StartCoroutine(GetRequest(baseAPIurl, "/icons/GetIcon" + query, GetIcon_Client, conn));
    }

    [TargetRpc]
    private void GetIcon_Client(NetworkConnection conn, string response)
    {
        // Handle response for getting icons
        Debug.Log("Icons: " + response);
    }

    // ------------------------ Achievements Endpoints ------------------------
    public void AddPlayerAchievement(int achievementId, string username, NetworkConnectionToClient conn)
    {
        AddPlayerAchievement_Server(achievementId, username, conn);
    }
    [Command]
    private void AddPlayerAchievement_Server(int achievementId, string username, NetworkConnectionToClient conn)
    {
        StartCoroutine(PostRequest<object>(baseAPIurl, $"/achievements/AddPlayerAchievement?achievement_id={achievementId}&username={username}", AddPlayerAchievement_Client, null, conn));
    }
    [TargetRpc]
    private void AddPlayerAchievement_Client(NetworkConnectionToClient conn, int code)
    {
        if (code == 201)
        {
            Debug.Log("Achievement added successfully");
        }
        else
        {
            Debug.Log("Achievement failed to add!");
        }
    }

    public void GetPlayerAchievements(string username, NetworkConnectionToClient conn)
    {
        GetPlayerAchievements_Server(username, conn);
    }

    [Command]
    private void GetPlayerAchievements_Server(string username, NetworkConnectionToClient conn)
    {
        StartCoroutine(GetRequest<AchievementOut[]>(baseAPIurl, "/achievements/GetPlayerAchievements?username=" + username, GetPlayerAchievements_Client, conn));
    }

    [TargetRpc]
    private void GetPlayerAchievements_Client(NetworkConnectionToClient conn, AchievementOut[] achievements)
    {
        if (achievements != null && achievements.Length > 0)
        {
            Debug.Log("Achievements found!");
            foreach (AchievementOut achievement in achievements)
            {
                Debug.Log("Achievement id: " + achievement.achieve_id);
            }
        }
        else
        {
            Debug.Log("No Achievements found!");
        }
    }

    public void GetAllAchievements(int? id, NetworkConnectionToClient conn)
    {
        GetAllAchievements_Server(id, conn);
    }
    [Command]
    private void GetAllAchievements_Server(int? id, NetworkConnectionToClient conn)
    {
        string query = (id.HasValue) ? $"?id={id.Value}" : "";
        StartCoroutine(GetRequest<AchievementOut[]>(baseAPIurl, "/achievements/GetAllAchievements" + query, GetAllAchievements_Client, conn));
    }

    [TargetRpc]
    private void GetAllAchievements_Client(NetworkConnectionToClient conn, AchievementOut[] achievements)
    {
        if (achievements != null && achievements.Length > 0)
        {
            Debug.Log("Achievements found!");
            foreach (AchievementOut achievement in achievements)
            {
                Debug.Log("Achievement name: " + achievement.name);
            }
        }
        else
        {
            Debug.Log("No Achievements found!");
        }
    }

    // ------------------------ Game Endpoints ------------------------
    public void CreateMatch(string gamePass, NetworkConnectionToClient conn)
    {
        CreateMatch_Server(gamePass, conn);
    }
    [Command]
    private void CreateMatch_Server(string gamePass, NetworkConnectionToClient conn)
    {
        string query = (!string.IsNullOrEmpty(gamePass)) ? $"?game_pass={gamePass}" : "";
        StartCoroutine(PostRequest<object>(baseAPIurl, "/games/CreateMatch" + query, CreateMatch_Client, null, conn));
    }
    [TargetRpc]
    private void CreateMatch_Client(NetworkConnectionToClient conn, int code)
    {
        if (code == 200)
        {
            Debug.Log("Match created successfully!");
        }
        else
        {
            Debug.Log("Match cannot be created. Error: " + code);
        }
    }

    public void AddPlayerToMatch(AddPlayerToMatchRequest requestData, NetworkConnectionToClient conn)
    {
        AddPlayerToMatch_Server(requestData, conn);
    }
    [Command]
    private void AddPlayerToMatch_Server(AddPlayerToMatchRequest requestData, NetworkConnectionToClient conn)
    {
        StartCoroutine(PostRequest(baseAPIurl, "/games/AddPlayerToMatch", AddPlayerToMatch_Client, requestData, conn));
    }
    [TargetRpc]
    private void AddPlayerToMatch_Client(NetworkConnectionToClient conn, int code)
    {
        if (code == 200)
        {
            Debug.Log("Player added to match successfully");
        }
        else
        {
            Debug.Log("Player cannot be added to match. Error: " + code);
        }
    }

    public void ChangePlayerRole(ChangePlayerRoleRequest requestData, NetworkConnectionToClient conn)
    {
        ChangePlayerRole_Server(requestData, conn);
    }
    [Command]
    private void ChangePlayerRole_Server(ChangePlayerRoleRequest requestData, NetworkConnectionToClient conn)
    {
        StartCoroutine(PutRequest(baseAPIurl, "/games/ChangePlayerRole", ChangePlayerRole_Client, requestData, conn));
    }

    [TargetRpc]
    private void ChangePlayerRole_Client(NetworkConnectionToClient conn, int code)
    {
        if (code == 200)
        {
            Debug.Log("Role changed successfully!");
        }
        else
        {
            Debug.Log("Role cannot be changed. Error " + code);
        }
    }

    public void EndGame(EndGameRequest requestData, NetworkConnectionToClient conn)
    {
        EndGame_Server(requestData, conn);
    }
    [Command]
    private void EndGame_Server(EndGameRequest requestData, NetworkConnectionToClient conn)
    {
        StartCoroutine(PutRequest(baseAPIurl, "/games/EndGame", EndGame_Client, requestData, conn));
    }

    [TargetRpc]
    private void EndGame_Client(NetworkConnectionToClient conn, int code)
    {
        if (code == 200)
        {
            Debug.Log("Game ended successfully");
        }
        else
        {
            Debug.Log("Game cannot be ended. Error: " + code);
        }
    }

    // ------------------------ Helper Methods ------------------------

    private IEnumerator GetRequest<T>(string uri, string endpoint, System.Action<NetworkConnectionToClient, T> handler, NetworkConnectionToClient conn = null)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + endpoint))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                try
                {
                    T data = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    handler(conn, data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing JSON: {e.Message}");
                    Debug.LogError("JSON Response: " + webRequest.downloadHandler.text);
                }
            }
        }
    }
    private IEnumerator GetRequest(string uri, string endpoint, ResponseHandler handler, NetworkConnection conn = null)
    {

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

    private IEnumerator PostRequest<T>(string uri, string endpoint, PostResponseHandler handler, T? postData, NetworkConnectionToClient conn) where T : class
    {
        handler(conn, 200);//(int)www.responseCode);
        string json = (postData != null) ? JsonUtility.ToJson(postData) : "";
        using (UnityWebRequest www = new UnityWebRequest(uri + endpoint, "POST"))
        {
            if (postData != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

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
                if (www.responseCode == 201 || www.responseCode == 200)
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

    private IEnumerator PutRequest<T>(string uri, string endpoint, PutResponseHandler handler, T putData, NetworkConnectionToClient conn)
    {
        string json = (putData != null) ? JsonUtility.ToJson(putData) : "";
        using (UnityWebRequest www = new UnityWebRequest(uri + endpoint, "PUT"))
        {
            if (putData != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
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
                if (www.responseCode == 201 || www.responseCode == 200)
                {
                    //Debug.LogError("PUT Request Successful!");
                }
                else
                {
                    Debug.LogError("Error in PUT response! Error code:" + www.responseCode);
                }
                Debug.Log("Response: " + responseText);
            }
            handler(conn, (int)www.responseCode);
        }
    }

    // ------------------------ Data Classes ------------------------
    [Serializable]
    public class PlayerBase
    {
        public string username;
        public string name;
        public string surname;
        public string gender;
        public string b_date;
        public int? age;
        public string address;
        public string email;
        public string password;
        public int? icon_id;
    }

    [Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class PlayerOut
    {
        public string username;
        public string name;
        public string surname;
        public string gender;
        public string b_date;
        public int? age;
        public string address;
        public string email;
        public int? icon_id;
    }

    [Serializable]
    public class AchievementOut
    {
        public string name;
        public string description;
        public int achieve_id;
    }

    [Serializable]
    public class AddPlayerToMatchRequest
    {
        public string username;
        public int game_id;
        public string team;
        public string game_pass;
    }
    [Serializable]
    public class ChangePlayerRoleRequest
    {
        public string username;
        public int game_id;
        public int round;
        public string role;
    }

    [Serializable]
    public class EndGameRequest
    {
        public int game_id;
        public string team;
        public string win_or_lose;
    }
    // ------------------------ Unity Methods ------------------------

    void Start()
    {
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
    }

    void Update()
    {

    }
}