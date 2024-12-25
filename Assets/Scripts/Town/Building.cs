using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField]
    private BuildingData buildingData;
    [SerializeField]
    private GameObject resourceSpawnPointObject;
    [SerializeField]
    private float spawnRadius = 1.0f;

    private PlayerNetworking player;
    private GameObject playerCharacter;

   // private NetworkConnectionToClient connectionToClient;
    private float currentHp;
    private bool isFunctional = false;
    private bool isColliding = false;
    private bool isOutOfBounds = false;
    private bool isBuildingMode = false;
    private bool isFarFromPlayer = false;

    private string hash;

    float maxDistanceToPlayerToBuild = 5f;
    void Awake()
    {
        //player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        currentHp = buildingData.hp;
    }

    private IEnumerator ProduceResource()
    {
        yield return new WaitForSeconds(buildingData.producingTimeSecs);
        
        Instantiate(buildingData.spawnResourcePrefab, resourceSpawnPointObject.transform.position + (Vector3)Random.insideUnitCircle*spawnRadius, buildingData.spawnResourcePrefab.transform.rotation); 
        //ResourceUpdateMsg msg = new ResourceUpdateMsg(player.synchronizedPlayerGameData.matchPtr.matchID, player.synchronizedPlayerGameData.teamNumber, buildingData.producingAmount, buildingData.resourceProduces);
        //GameManager.instance.CMDUpdateResource(msg);
        StartCoroutine(ProduceResource());


        // Play animation and spawn resource on ground??? for pawn to pick it up and bring to base or for player himself
    }
    public void SetHash(string hash)
    {
        this.hash = hash;
    }
    public string GetHash()
    {
        return hash;
    }
    public void DamageBy(int damage)
    {
        currentHp -= damage;
        Debug.Log("Building  " + hash + " 's health is " + currentHp);
    }

    public void SetPlayer(PlayerNetworking player)
    {
        this.player = player; 
        //connectionToClient = player.GetComponent<NetworkIdentity>().connectionToClient;
    }
    public void SetFunctional(bool isFunctional) 
    { 
        this.isFunctional = isFunctional; 
        if (isFunctional 
            && player.GetUserData().username == LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().GetUserData().username 
            && buildingData.resourceProduces != Resource.NONE)
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

    public bool IsFarFromPlayer()
    {
        return isFarFromPlayer;
    }

    public BuildingData GetBuildingData()
    {
        return buildingData;
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
        if (isBuildingMode && (collision.gameObject.tag == "Building" || collision.gameObject.tag == "Player"))
        {
            isColliding = true;
            //PaintItRed();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isBuildingMode && (collision.gameObject.tag == "Building" || collision.gameObject.tag == "Player"))
        {
            isColliding = false;
            //PaintItGreen();

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isBuildingMode && (collision.gameObject.tag == "Building" || collision.gameObject.tag == "Player"))
        {
            isColliding = true;
            //PaintItRed();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Town")
        {
           // PaintItRed();
            isOutOfBounds = true;    
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBuildingMode && collision.gameObject.tag == "Town")
        {
            //PaintItGreen();
            isOutOfBounds = false;     
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = LocalStateManager.instance.localPlayer.gameObject.GetComponentInChildren<CharacterControllerBase>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Distance to player check
        if (isBuildingMode)
        {
            float distanceToPlayer = ((Vector2)(transform.position - playerCharacter.transform.position)).magnitude;
            Debug.Log("Distance to player is " + distanceToPlayer);
            if (distanceToPlayer > maxDistanceToPlayerToBuild) isFarFromPlayer = true;
            else isFarFromPlayer = false;

            if (isColliding || isFarFromPlayer || isOutOfBounds) PaintItRed();
            else PaintItGreen();
        }
        if (currentHp <= 0)
        {
            if (buildingData.buildingName == "TownHall")
            {
                GameManager.instance.CMDDoGameOver(LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID);
                return;
            }
            GameManager.instance.buildingManager.CMDDestroyBuilding(player.synchronizedPlayerGameData.matchPtr.matchID, hash);
        }
        //if (isFarFromPlayer) PaintItRed();
        //else PaintItGreen();
    }
}
