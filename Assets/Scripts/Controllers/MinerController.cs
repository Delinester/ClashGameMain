using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerController : CharacterControllerBase
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}
    private bool isAttacking = false;

    public bool IsAttacking() { return isAttacking; }

    private int mineralOreCount = 0;
    private int goldOrdeCount = 0;

    private float collectItemsRadius = 5f;

    private int damage = 25;

    public int GetMineralOreCount() { return mineralOreCount; }
    public int GetGoldOreCount() {  return goldOrdeCount; }
    public void AddMineralOre(int amountToAdd) {  mineralOreCount += amountToAdd;}
    public void AddGoldOreCount(int amountToAdd) { goldOrdeCount += amountToAdd;}

    public int GetDamage() { return damage; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger " + collision.gameObject.tag);
        if (collision.gameObject.tag == "ToMineTeleporter")
        {
            Debug.Log("Triggerrrr " + collision.gameObject.tag);
            int teamNum = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber;
            Vector2 mineLocation = GameManager.instance.mineManager.GetMineLocation(teamNum) + (Vector3)(new Vector2(2,2));
            //GetComponent<Rigidbody2D>().MovePosition(mineLocation);
            transform.position = mineLocation;
        }
        else if (collision.gameObject.tag == "RoomBoundary")
        {
            CameraController.instance.SetBounds(collision.bounds);
        }
        else if (collision.gameObject.tag == "Collectible")
        {
            ResourceScript resource = collision.gameObject.GetComponent<ResourceScript>();
            ResourceUpdateMsg msg = new ResourceUpdateMsg(
                    networkPlayer.synchronizedPlayerGameData.matchPtr.matchID,
                    networkPlayer.synchronizedPlayerGameData.teamNumber,
                    resource.GetResourceAmount(),
                    resource.GetResourceType());
            GameManager.instance.CMDUpdateResource(msg);
            Destroy(collision.gameObject);            
        }
        else if (collision.gameObject.tag == "TeleportToShack")
        {
            Vector2 pos = GameManager.instance.GetMineShackLocation(LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber);
            transform.position = pos + new Vector2(6, -3);
        }
    }

    private void AttractResources()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, collectItemsRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "Collectible")
            {
                ResourceScript resource = collider.gameObject.GetComponent<ResourceScript>();
                if (!resource.IsAttractedAlready())
                {
                    resource.AttractToPoint(transform.position);                    
                }
            }
        }
    }

    private IEnumerator AttackDelayCoro()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Debug.Log("Attack trigger!");
            animator.SetTrigger("Attack");
            isAttacking = true;
            StartCoroutine(AttackDelayCoro());
        }
        AttractResources();
    }
}
