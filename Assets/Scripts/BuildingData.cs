using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewData", menuName = "BuildingData")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite buildingSprite;
    public GameObject buildingPrefab;
    public Resource resourceProduces = Resource.NONE;
    public float hp;
    public int costGold;
    public int producingAmount = 0;
    public int producingTimeSecs;
    public int maxQuantity;
}
