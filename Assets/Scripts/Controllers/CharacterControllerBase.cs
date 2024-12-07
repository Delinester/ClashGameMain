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


    protected PlayerNetworking networkPlayer;
    protected Animator animator;
    protected NetworkAnimator networkAnimator;

    private float scaleX;


    // Start is called before the first frame update
    virtual protected void Start()
    {
        networkPlayer = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        networkAnimator = GetComponent<NetworkAnimator>();
        animator = GetComponent<Animator>();
        scaleX = transform.localScale.x;
    }

    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Entered collision with " + collision.gameObject.tag);
        
        //if (collision.gameObject.tag == "Collectible" && resourcesInHand < maxResourcesInHand)
        //{
        //    ResourceScript resource = collision.gameObject.GetComponent<ResourceScript>();
        //    if (resourceTypeInHand == Resource.NONE)
        //    {
        //        resourceTypeInHand = resource.GetResourceType();
        //        UpdateResourceIcon(resource.GetResourceSprite());
        //    }
        //    if (resource.GetResourceType() == resourceTypeInHand)
        //    {
        //        resourcesInHand++;
        //        UpdateResourceCountText(resourcesInHand);
        //        Destroy(collision.gameObject);
        //    }            
        //}
    }      

    protected void MoveCharacter()
    {
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");


        //networkAnimator.animator.SetFloat("MovingSpeed", Mathf.Abs(movementX) + Mathf.Abs(movementY));

        GetComponent<Rigidbody2D>().MovePosition((Vector2)(gameObject.transform.position) + new Vector2(movementX, movementY) * moveSpeed * Time.deltaTime);
        animator.SetFloat("MoveSpeed", Mathf.Abs(movementX) + Mathf.Abs(movementY));
        if (movementX < 0) transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        else if (movementX > 0) transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        
    }

    virtual protected void Update()
    {
        MoveCharacter();
    }
}
