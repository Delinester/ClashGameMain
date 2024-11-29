using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class CharacterControllerBase : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField] LayerMask layerMask;
    [SerializeField]
    private float collectItemsRadius = 1.5f;

    [SerializeField]
    private Image resourceInHandImage;
    [SerializeField]
    private TextMeshProUGUI resourceInHandCountText;

    private int resourcesInHand = 0;
    private const int maxResourcesInHand = 5;
    private Resource resourceTypeInHand = Resource.NONE;

    private PlayerNetworking networkPlayer;
    private float rayLen = 0.5f;

    private NetworkAnimator networkAnimator;

    // Start is called before the first frame update
    void Start()
    {
        UpdateResourceCountText(resourcesInHand);
        networkPlayer = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Collectible" && resourcesInHand < maxResourcesInHand)
        {
            ResourceScript resource = collision.gameObject.GetComponent<ResourceScript>();
            if (resourceTypeInHand == Resource.NONE)
            {
                resourceTypeInHand = resource.GetResourceType();
                UpdateResourceIcon(resource.GetResourceSprite());
            }
            if (resource.GetResourceType() == resourceTypeInHand)
            {
                resourcesInHand++;
                UpdateResourceCountText(resourcesInHand);
                Destroy(collision.gameObject);
            }            
        }
    }

    private void UpdateResourceIcon(Sprite iconSprite)
    {
        resourceInHandImage.sprite = iconSprite;
    }

    private void UpdateResourceCountText(int count)
    {
        resourceInHandCountText.text = count.ToString();
    }

    private void CheckBuildingIfTownHallAndDepositResources(Collider2D collider)
    {
        BuildingData buildingData = collider.gameObject.GetComponent<Building>().GetBuildingData();
        if (buildingData.buildingName == "TownHall")
        {
            if (Input.GetKeyDown(KeyCode.E) && resourcesInHand != 0 && resourceTypeInHand != Resource.NONE)
            {
                Debug.Log("Depositing resource in amount " + resourcesInHand);
                ResourceUpdateMsg msg = new ResourceUpdateMsg(
                    networkPlayer.synchronizedPlayerGameData.matchPtr.matchID, 
                    networkPlayer.synchronizedPlayerGameData.teamNumber, 
                    resourcesInHand, resourceTypeInHand);
                GameManager.instance.CMDUpdateResource(msg);

                resourceTypeInHand = Resource.NONE;
                resourcesInHand = 0;
                UpdateResourceCountText(0);
                UpdateResourceIcon(null);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");
        
        RaycastHit2D hitLeft = Physics2D.Raycast(gameObject.transform.position, transform.TransformDirection(Vector2.left), rayLen, layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(gameObject.transform.position, transform.TransformDirection(Vector2.right), rayLen, layerMask);
        RaycastHit2D hitUp = Physics2D.Raycast(gameObject.transform.position, transform.TransformDirection(Vector2.up), rayLen, layerMask);
        RaycastHit2D hitDown = Physics2D.Raycast(gameObject.transform.position, transform.TransformDirection(Vector2.down), rayLen, layerMask);

        Ray2D[] rays = new Ray2D[4];
        rays[0] = new Ray2D(gameObject.transform.position, transform.TransformDirection(Vector2.left));
        rays[1] = new Ray2D(gameObject.transform.position, transform.TransformDirection(Vector2.right));
        rays[2] = new Ray2D(gameObject.transform.position, transform.TransformDirection(Vector2.up));
        rays[3] = new Ray2D(gameObject.transform.position, transform.TransformDirection(Vector2.down));

        foreach (Ray2D ray in rays)
        {
            Debug.DrawRay(ray.origin, ray.direction * rayLen, Color.red);
        }

        if (hitLeft.collider && hitLeft.collider.gameObject.tag == "Building" && hitLeft.distance < rayLen && movementX < 0)
        {            
            movementX = 0;
            CheckBuildingIfTownHallAndDepositResources(hitLeft.collider);
        }
        if (hitRight.collider && hitRight.collider.gameObject.tag == "Building" && hitRight.distance < rayLen && movementX > 0)
        {
            movementX = 0;
            CheckBuildingIfTownHallAndDepositResources(hitRight.collider);
        }
        if (hitUp.collider && hitUp.collider.gameObject.tag == "Building" && hitUp.distance < rayLen && movementY > 0)
        {
            movementY = 0;
            CheckBuildingIfTownHallAndDepositResources(hitUp.collider);
        }
        if (hitDown.collider && hitDown.collider.gameObject.tag == "Building" && hitDown.distance < rayLen && movementY < 0)
        {
            movementY = 0;
            CheckBuildingIfTownHallAndDepositResources(hitDown.collider);
        }

        networkAnimator.animator.SetFloat("MovingSpeed", Mathf.Abs(movementX) + Mathf.Abs(movementY));

        transform.Translate(new Vector2(movementX * moveSpeed * Time.deltaTime, movementY * moveSpeed * Time.deltaTime));


        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, collectItemsRadius);
        
        int attractedAmount = 0;
        foreach (Collider2D collider in colliders)
        {           
            if (collider.gameObject.tag == "Collectible")
            {
                if (resourcesInHand + attractedAmount >= maxResourcesInHand)
                {
                    break;
                }
                ResourceScript resource = collider.gameObject.GetComponent<ResourceScript>();
                if ((resource.GetResourceType() == resourceTypeInHand || resourceTypeInHand == Resource.NONE) && !resource.IsAttractedAlready())
                {
                    resource.AttractToPoint(transform.position);
                    attractedAmount++;
                }
            }
        }

    }
}
