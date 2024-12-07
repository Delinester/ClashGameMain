using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    private MinerController minerController;
    [SerializeField]
    private ParticleSystem collisionParticleSystem;

    private bool isAlreadyCollided = false;
    private void Awake()
    {
        minerController = GetComponentInParent<MinerController>();
       // collisionParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Pickaxe collided with " + collision.gameObject.tag);
        if (minerController.IsAttacking() && collision.gameObject.tag == "Destructible" && !isAlreadyCollided)
        {
            Debug.Log("Pickaxe collided with destructible");
            collisionParticleSystem.Play();
            isAlreadyCollided = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!minerController.IsAttacking()) isAlreadyCollided = false;
    }
}
