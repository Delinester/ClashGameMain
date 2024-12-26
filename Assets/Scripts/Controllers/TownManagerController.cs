using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class TownManagerController : CharacterControllerBase
{
    [SerializeField]
    private Image resourceInHandImage;
    [SerializeField]
    private TextMeshProUGUI resourceInHandCountText;
    [SerializeField]
    private GameObject depositResourcesUI;

    [SerializeField]
    private float collectItemsRadius = 1.5f;


    private int resourcesInHand = 0;
    private const int maxResourcesInHand = 5;
    private Resource resourceTypeInHand = Resource.NONE;

    private Vector2 townHallPos = new Vector2(9999,9999);


    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        UpdateResourceCountText(resourcesInHand);
    }

    private void UpdateResourceIcon(Sprite iconSprite)
    {
        resourceInHandImage.sprite = iconSprite;
    }

    private void UpdateResourceCountText(int count)
    {
        resourceInHandCountText.text = count.ToString();
    }


    private void OnTriggerEnter2D(Collider2D collision)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }
    void AttractNearbyResources()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, collectItemsRadius);

        int attractedAmount = 0;
        bool townHallNearby = false;
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
            else if (collider.gameObject.tag == "Building" && collider.gameObject.GetComponent<Building>().GetBuildingData().buildingName == "TownHall")
            {
                townHallNearby = true;
                CheckBuildingIfTownHallAndDepositResources(collider);                
            }
        }
        if (!townHallNearby) depositResourcesUI.SetActive(false);
    }

    private void CheckBuildingIfTownHallAndDepositResources(Collider2D collider)
    {
        BuildingData buildingData = collider.gameObject.GetComponent<Building>().GetBuildingData();
        if (buildingData.buildingName == "TownHall")
        {
            depositResourcesUI.SetActive(true);
            townHallPos = collider.transform.position;

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
    override protected void Update()
    {
        base.Update();
        AttractNearbyResources();       

    }
}
