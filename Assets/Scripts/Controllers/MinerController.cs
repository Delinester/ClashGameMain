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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger " + collision.gameObject.tag);
        if (collision.gameObject.tag == "ToMineTeleporter")
        {
            Debug.Log("Triggerrrr " + collision.gameObject.tag);
            int teamNum = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber;
            Vector2 mineLocation = GameManager.instance.mineManager.GetMineLocation(teamNum);
            //GetComponent<Rigidbody2D>().MovePosition(mineLocation);
            transform.position = mineLocation;
        }
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
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking") && isAttacking) isAttacking = false;
    }
}
