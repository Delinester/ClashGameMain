using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopAI : CharacterControllerBase
{
    [SerializeField]
    private TroopData troopData;

    private int teamNumber;

    private NavMeshAgent agent;

    bool canAttack = true;


    private IEnumerator AttackCoro(Building building)
    {
        animator.SetTrigger("Attack");
        canAttack = false;
        building.DamageBy(troopData.damage);
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    public void SetTeamNumber(int teamNumber)
    {
        this.teamNumber = teamNumber;
    }

    public void SetMovingSpeed(float speed)
    {
        agent.speed = speed;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (!IsPuppet())
        {
            agent.SetDestination(GameManager.instance.GetTownTeamPosition(teamNumber == 1 ? 2 : 1));
        }
    }


    // Start is called before the first frame update
    protected override void MoveCharacter()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        bool areThereBuildings = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "Building")
            {
                agent.SetDestination(collider.gameObject.transform.position);
                areThereBuildings = true;
                if (canAttack)
                    StartCoroutine(AttackCoro(collider.GetComponent<Building>()));
                //Debug.Log("Velocity is " + agent.velocity);
            }
        }

        if (!areThereBuildings)
        {
            agent.SetDestination(GameManager.instance.GetTownTeamPosition(teamNumber == 1 ? 2 : 1));
        }
    }

    protected override void Update()
    {
        base.Update();
        if (agent.velocity.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (agent.velocity.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
