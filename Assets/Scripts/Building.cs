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
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
