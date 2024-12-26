using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopAI : CharacterControllerBase
{
    [SerializeField]
    private TroopData troopData;

    private float currentHp;

    private int teamNumber;

    private NavMeshAgent agent;

    private bool canAttack = true;

    private Bounds townBounds;
    private bool isRandomPointChosen = false;
    private Vector2 chosenPoint;

    private IEnumerator AttackCoro(Building building)
    {
        animator.SetTrigger("Attack");
        canAttack = false;
        building.DamageBy(troopData.damage);
        DamageTroop(troopData.damage);
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Troop collided with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Town")
        {
            townBounds = collision.gameObject.GetComponent<Collider2D>().bounds;
            Debug.Log($"Bounds are {townBounds.min.x} {townBounds.max.x} {townBounds.min.y} {townBounds.max.y}");
            agent.SetDestination(GetRandomPoint());
            isRandomPointChosen = true;
        }
    }
    private Vector2 GetRandomPoint()
    {
        if (townBounds.max.x == 0)
        {
            return GameManager.instance.GetTownTeamPosition(teamNumber == 1 ? 2 : 1);
        }
        float randX = Random.Range(townBounds.min.x, townBounds.max.x);
        float randY = Random.Range(townBounds.min.y, townBounds.max.y);
        chosenPoint = new Vector2(randX, randY);
        Debug.Log("Random point is " + chosenPoint);
        return chosenPoint;
    }
    public void SetTeamNumber(int teamNumber)
    {
        this.teamNumber = teamNumber;
    }

    public void SetMovingSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void DamageTroop(int damage)
    {
        currentHp -= damage;
        Debug.Log($"Troop {GetHash()} current hp is {currentHp}");
    }

    public void ResetPath()
    {
        agent.ResetPath();
        agent.SetDestination(agent.destination);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentHp = troopData.hp;
        if (!IsPuppet())
        {
            ResetPath();
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
                isRandomPointChosen = false;

                Vector3 randomOffset = new Vector3(0, Random.Range(-1f, 1f), 0);
                agent.SetDestination(collider.gameObject.transform.position + randomOffset);
                areThereBuildings = true;
                if (canAttack)
                    StartCoroutine(AttackCoro(collider.GetComponent<Building>()));
                //Debug.Log("Velocity is " + agent.velocity);
            }
        }

        if (!areThereBuildings && !isRandomPointChosen)
        {
            //agent.SetDestination(GameManager.instance.GetTownTeamPosition(teamNumber == 1 ? 2 : 1));
            agent.SetDestination(GetRandomPoint());
            isRandomPointChosen = true;
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

        if (Vector2.Distance(chosenPoint, transform.position) < 2f)
        {
            isRandomPointChosen = false;
        }

        if (currentHp <= 0)
        {
            string matchID = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.matchPtr.matchID;
            GameManager.instance.CMDDestroyPuppetOnClients(matchID, GetHash());

            Destroy(gameObject);
        }
    }
}
