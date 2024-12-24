using Mirror.BouncyCastle.Asn1.Mozilla;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

[System.Serializable]
public class Troop
{
    [SerializeField]
    public TroopData data;
    [SerializeField]
    public int count;
}

[System.Serializable]
public class Army
{
    [SerializeField]
    public Resource[] troopTypes;
    [SerializeField]
    public int[] counts;
    [SerializeField]
    public int ownerTeamNum = 0;

    public Army()
    {
        troopTypes = null;
        counts = null;
    }


    public Army(Troop[] troops)
    {
        this.troopTypes = new Resource[troops.Length];
        this.counts = new int[troops.Length];

        for (int i = 0; i < troopTypes.Length; i++)
        {
            this.troopTypes[i] = troops[i].data.troopType;
            this.counts[i] = troops[i].count;
        }
    }
}

public class WorldMapArmyAI : CharacterControllerBase
{
    [SerializeField]
    private TextMeshProUGUI armyCountText;
    [SerializeField]
    private GameObject[] stars;

    private bool isMovingSomewhere = false;
    private Vector2 destinationPoint = Vector2.zero;

    private int ownerTeamNum = 0;

    private bool isInCombat = false;

    private Troop[] troops = new Troop[3];
    protected override void MoveCharacter()
    {
        Vector2 currentPos = this.transform.position;
        if (Vector2.Distance(destinationPoint, currentPos) < 0.1f)
        {
            isMovingSomewhere = false;
        }

        if (isMovingSomewhere)
        {
            Vector2 currentPosition = this.transform.position;
            Vector2 targetDirection = (destinationPoint - currentPosition).normalized;
            GetComponent<Rigidbody2D>().MovePosition(currentPosition + targetDirection * GetMoveSpeed() * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Army" && !collision.GetComponent<WorldMapArmyAI>().IsInCombat() && GetOwnerTeamNum() != collision.GetComponent<WorldMapArmyAI>().GetOwnerTeamNum())
        {
            isInCombat = true;
            collision.GetComponent<WorldMapArmyAI>().SetIsInCombat(true);
            GameManager.instance.battlesManager.EngageInBattle(this, collision.GetComponent<WorldMapArmyAI>());
            MoveToPoint(transform.position);
        }
    }

    public override void SetHash(string hash)
    {
        base.SetHash(hash);
        GameManager.instance.battlesManager.AddNewArmy(this);
    }

    public void SetBaseOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", 1.5f);
    }

    public void SetThickOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", 5f);
    }

    public void SetNoOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", 0);
    }

    public void SetTroopsInArmy(Troop[] troops)
    {
        int armySize = 0;
        for (int i = 0; i < troops.Length; i++)
        {
            this.troops[i] = troops[i];
            armySize += troops[i].count;
        }
        armyCountText.text = armySize.ToString();
        SetStars(GetMaxTroopWeight());
    }

    public void SetStars(int amount)
    {
        foreach (GameObject obj in stars) obj.SetActive(false);

        for (int i = 0; i < amount; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void SetArmyOwnerTeam(int teamNum)
    {
        this.ownerTeamNum = teamNum;
    }

    public int GetOwnerTeamNum()
    {
        return ownerTeamNum;
    }

    public Troop[] GetTroopsInArmy()
    {
        return troops;
    }

    public bool IsInCombat()
    {
        return isInCombat;
    }

    public void SetIsInCombat(bool isInCombat)
    {
        this.isInCombat = isInCombat;
    }

    public int GetArmyPoints()
    {
        int points = 0;
        foreach (Troop troop in GetTroopsInArmy())
        {
            points += troop.data.weightInBattle * troop.count;
        }
        return points;
    }

    public int GetMaxTroopWeight()
    {
        int maxWeight = 0;
        foreach (Troop t in GetTroopsInArmy())
        {
            int troopMax = t.count != 0 ? t.data.weightInBattle : 0;
            maxWeight = Mathf.Max(maxWeight, troopMax);
        }
        return maxWeight;
    }

    public int GetArmyGoldCost()
    {
        int cost = 0;
        foreach (Troop t in GetTroopsInArmy())
        {
            cost += t.count * t.data.goldCost;
        }
        return cost;
    }

    public int GetArmyFoodCost()
    {
        int cost = 0;
        foreach (Troop t in GetTroopsInArmy())
        {
            cost += t.count * t.data.meatCost;
        }
        return cost;
    }

    public void MoveToPoint(Vector2 point)
    {
        isMovingSomewhere = true;
        destinationPoint = point;
    }

    void Awake()
    {
    }
}
