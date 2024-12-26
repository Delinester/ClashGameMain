using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private Camera m_camera;

    [SerializeField]
    private float zoomSpeed = 50f;
    [SerializeField]
    private float moveSpeed = 10f;

    private Bounds currentBoundaries;

    private Vector3 mousePosition;
    private GameObject playerToFollow;
    private GameRole currentRole;

    private bool isFollowingPlayer = true;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        m_camera = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {        
        if (LocalStateManager.instance.localPlayerCharacter)
            playerToFollow = LocalStateManager.instance.localPlayerCharacter;
        currentRole = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.role;
        if (playerToFollow)
            m_camera.transform.position = new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, m_camera.transform.position.z);
        // Set camera position to the location according to player's role!
        //Vector3 townPos = GameManager.instance.GetTownTeamPosition(LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber);
        //m_camera.transform.position = new Vector3(townPos.x, townPos.y, m_camera.transform.position.z);
    }

    public void SetBounds(Bounds bounds) 
    { 
        currentBoundaries = bounds; 
    }

    public void SetFollowingPlayer(bool isFollowing)
    {
        isFollowingPlayer = isFollowing;
    }

    public void SetPlayerToFollow(GameObject player)
    {
        playerToFollow = player;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (playerToFollow)
        {
            float height = m_camera.orthographicSize * 2; // Total height
            float width = height * m_camera.aspect;       // Total width (aspect ratio = width/height)

            Vector3 cameraPosition = m_camera.transform.position;

            // Calculate bounds
            float left = cameraPosition.x - width / 2;
            float right = cameraPosition.x + width / 2;
            float top = cameraPosition.y + height / 2;
            float bottom = cameraPosition.y - height / 2;

            //Debug.Log($"Left: {left}, Right: {right}, Top: {top}, Bottom: {bottom}" + " BOUNDS " + currentBoundaries.min + " max " + currentBoundaries.max);
            Vector3 newPos = transform.position;
            if (!isFollowingPlayer)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    mousePosition = Input.mousePosition;
                }

                if (Input.GetKey(KeyCode.Mouse1))
                {
                    Vector3 currentMousePosition = Input.mousePosition;
                    Vector3 delta = currentMousePosition - mousePosition;
                    float x = m_camera.transform.position.x - delta.x * moveSpeed * Time.deltaTime;
                    float y = m_camera.transform.position.y - delta.y * moveSpeed * Time.deltaTime;
                    newPos = new Vector3(x, y, m_camera.transform.position.z);
                }
            }
            else
                newPos = new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, m_camera.transform.position.z);

            if (currentBoundaries.min.x != 0)
            {
                //if (left < currentBoundaries.min.x) newPos.x = currentBoundaries.min.x;
                // if (right > currentBoundaries.max.x) newPos.x = currentBoundaries.max.x;
                if (newPos.y + height / 2 > currentBoundaries.max.y) newPos.y = currentBoundaries.max.y - height / 2;
                if (newPos.y - height / 2 < currentBoundaries.min.y) newPos.y = currentBoundaries.min.y + height / 2;
                if (newPos.x + width / 2 > currentBoundaries.max.x) newPos.x = currentBoundaries.max.x - width / 2;
                if (newPos.x - width / 2 < currentBoundaries.min.x) newPos.x = currentBoundaries.min.x + width / 2;
                //if (bottom < currentBoundaries.min.y) newPos.y = currentBoundaries.min.y;
            }
            m_camera.transform.position = newPos;
            // Zooming
            float mouseScrolDelta = Input.mouseScrollDelta.y;
            float ortoSize = m_camera.orthographicSize;
            if (mouseScrolDelta < 0f) ortoSize += zoomSpeed * Time.deltaTime;
            else if (mouseScrolDelta > 0f) ortoSize -= zoomSpeed * Time.deltaTime;

            if (ortoSize > 4f) ortoSize = 4;
            else if (ortoSize < 3f) ortoSize = 3;

            m_camera.orthographicSize = ortoSize;

            // Camera movement
            //if (Input.GetMouseButtonDown(1))
            //{
            //    mousePosition = Input.mousePosition;
            //}

            //if (Input.GetKey(KeyCode.Mouse1))
            //{
            //    Vector3 currentMousePosition = Input.mousePosition;
            //    Vector3 delta = currentMousePosition - mousePosition;
            //    float x = m_camera.transform.position.x - delta.x * moveSpeed * Time.deltaTime;
            //    float y = m_camera.transform.position.y - delta.y * moveSpeed * Time.deltaTime;
            //    m_camera.transform.position = new Vector3(x, y, m_camera.transform.position.z);
            //}
            mousePosition = Input.mousePosition;
            // TODO : ADD CONSTRAINS!!!!!!
        }
    }
}
