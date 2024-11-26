using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerBase : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private NetworkAnimator networkAnimator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");

        transform.Translate(new Vector2(movementX * moveSpeed * Time.deltaTime, movementY * moveSpeed * Time.deltaTime));
    }
}
