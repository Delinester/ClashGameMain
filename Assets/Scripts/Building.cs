using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField]
    private BuildingData buildingData;

    private PlayerNetworking player;
   // private NetworkConnectionToClient connectionToClient;
    private float currentHp;
    private bool isFunctional = false;
    private bool isColliding = false;
    private bool isOutOfBounds = false;
    private bool isBuildingMode = false;
    void Awake()
    {
        //player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        currentHp = buildingData.hp;
    }

    private IEnumerator ProduceResource()
    {
        yield return new WaitForSeconds(buildingData.producingTimeSecs);
        ResourceUpdateMsg msg = new ResourceUpdateMsg(player.synchronizedPlayerGameData.matchPtr.matchID, player.synchronizedPlayerGameData.teamNumber, buildingData.producingAmount, buildingData.resourceProduces);
        GameManager.instance.CMDUpdateResource(msg);
        StartCoroutine(ProduceResource());
        // Play animation and spawn resource on ground??? for pawn to pick it up and bring to base or for player himself
    }

    public void SetPlayer(PlayerNetworking player)
    {
        this.player = player; 
        //connectionToClient = player.GetComponent<NetworkIdentity>().connectionToClient;
    }
    public void SetFunctional(bool isFunctional) 
    { 
        this.isFunctional = isFunctional; 
        if (isFunctional && player.GetUserData().username == LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().GetUserData().username)
        {
            StartCoroutine(ProduceResource());
        }
    }

    public void SetBuildingMode(bool isBuildingMode)
    {
        this.isBuildingMode = isBuildingMode;
    }

    public bool IsColliding()
    {
        return isColliding;
    }

    public bool IsOutOfBounds()
    {
        return isOutOfBounds;
    }

    private void PaintItColor(float r, float g, float b, float a)
    {
        try
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
        }
        catch (MissingComponentException)
        {
            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = new Color(r, g, b, a);
            }
        }
    }

    private void PaintItRed()
    {
        PaintItColor(255, 0, 0, 255);
    }

    private void PaintItGreen()
    {
        PaintItColor(0, 255, 0, 255);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Building")
        {
            isColliding = true;
            PaintItRed();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Building")
        {
            isColliding = false;
            PaintItGreen();

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Building")
        {
            isColliding = true;
            PaintItRed();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Town")
        {
            PaintItRed();
            isOutOfBounds = true;    
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Town")
        {
            PaintItGreen();
            isOutOfBounds = false;     
        }
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
