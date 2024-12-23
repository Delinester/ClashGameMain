using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Troop
{
    public TroopData data;
    public int count;
}

public class WorldMapArmyAI : CharacterControllerBase
{
    [SerializeField]
    private TextMeshProUGUI armyCountText;

    private bool isMovingSomewhere = false;
    private Vector2 destinationPoint = Vector2.zero;

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

    public void SetTroopsInArmy(Troop[] troops)
    {
        int armySize = 0;
        for (int i = 0; i < troops.Length; i++)
        {
            this.troops[i] = troops[i];
            armySize += troops[i].count;
        }
        armyCountText.text = armySize.ToString();
    }

    public void MoveToPoint(Vector2 point)
    {
        isMovingSomewhere = true;
        destinationPoint = point;
    }
}
