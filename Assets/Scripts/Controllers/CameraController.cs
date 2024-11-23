using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera m_camera;

    [SerializeField]
    private float zoomSpeed = 50f;
    [SerializeField]
    private float moveSpeed = 10f;


    private Vector3 mousePosition;

    void Awake()
    {
        m_camera = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // Set camera position to the location according to player's role!
        Vector3 townPos = GameManager.instance.GetTownTeamPosition(LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber);
        m_camera.transform.position = new Vector3(townPos.x, townPos.y, m_camera.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Zooming
        float mouseScrolDelta = Input.mouseScrollDelta.y;
        float ortoSize = m_camera.orthographicSize;
        if (mouseScrolDelta < 0f) ortoSize += zoomSpeed * Time.deltaTime;
        else if (mouseScrolDelta > 0f) ortoSize -= zoomSpeed * Time.deltaTime;

        if (ortoSize > 5f) ortoSize = 5;
        else if (ortoSize < 3f) ortoSize = 3;

        m_camera.orthographicSize = ortoSize;

        // Camera movement
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
            m_camera.transform.position = new Vector3(x, y, m_camera.transform.position.z);
        }
        mousePosition = Input.mousePosition;
        // TODO : ADD CONSTRAINS!!!!!!
    }
}
