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

    string hash;

    protected PlayerNetworking networkPlayer;
    protected Animator animator;
    //protected NetworkAnimator networkAnimator;

    private Vector3 lastPosition;
    private Vector3 lastScale;

    private string ownerUsername;
    private string matchID;

    private float scaleX;
    private bool isPuppet = false;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        if (!isServer)
        {
            networkPlayer = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
           // networkAnimator = GetComponent<NetworkAnimator>();
            animator = GetComponent<Animator>();
            scaleX = transform.localScale.x;
            lastPosition = transform.position;
            lastScale = transform.localScale;


            PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
            ownerUsername = player.GetUserData().username;
            matchID = player.synchronizedPlayerGameData.matchPtr.matchID;

        }
    }

    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
       
    }

    public void SetHash(string hash)
    {
        this.hash = hash;
    }
    public string GetHash() { return hash; }

    [Client]
    private void CheckTransformChanges()
    {
        if (transform.position != lastPosition || transform.localScale != lastScale)
        {
            Debug.Log("Change of transform!!");
            GameManager.instance.CMDUpdateTransformCharacter(matchID, ownerUsername, hash, transform.position, transform.localScale);

            lastPosition = transform.position;
            lastScale = transform.localScale;
        }
    }    

    public void SetIsPuppet(bool isPuppet)
    {
        this.isPuppet = isPuppet;
    }
    public bool IsPuppet() { return this.isPuppet; }
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

    void FixedUpdate()
    {
        if (!isServer)
        {
            if (!isPuppet)
            {
                MoveCharacter();
            }
            
        }

    }

    virtual protected void Update()
    {
        if (!isPuppet && !isServer)
        {
            CheckTransformChanges();
        }
    }
}
