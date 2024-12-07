using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScript : MonoBehaviour
{
    [SerializeField]
    private float attractionSpeed = 6f;
    [SerializeField]
    private Resource resourceType;
    [SerializeField]
    private int resourceAmount;

    private bool isAttracted = false;
    private Vector2 attractionPoint;

    public bool IsAttractedAlready() { return isAttracted; }
    public void AttractToPoint(Vector2 point)
    {
        isAttracted = true;
        attractionPoint = point;
    }

    public Resource GetResourceType()
    {
        return resourceType;
    }

    public Sprite GetResourceSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public int GetResourceAmount()
    { return resourceAmount; }  

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // assign resource to hand
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttracted)
        {
            Vector2 direction = (attractionPoint - (Vector2)transform.position).normalized;
            transform.Translate(direction * attractionSpeed * Time.deltaTime);
        }
    }
}
