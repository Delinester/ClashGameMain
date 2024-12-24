using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewData", menuName = "TroopData")]

public class TroopData : ScriptableObject
{
    public string troopName;
    public int hp;
    public int damage;
    public int goldCost;
    public int meatCost;
    public int moveSpeed;
    public int trainTimeSeconds;
    public float attackSpeed;
    public int weightInBattle;
    public Sprite troopIcon;
    public GameObject prefab;
    public Resource troopType;
}
