using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    private MinerController minerController;
    [SerializeField]
    private ParticleSystem collisionParticleSystem;

    private void Awake()
    {
        minerController = GetComponentInParent<MinerController>();
       // collisionParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Pickaxe collided with " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Destructible")
        {
            Debug.Log("Pickaxe collided with destructible");
            collisionParticleSystem.Play();
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
