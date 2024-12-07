using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewData", menuName = "DestructibleObjectData")]
public class DestructibleObjectData : ScriptableObject
{
    public string objName;
    public GameObject[] prefabs;
    public int spawnChanceRange;
    public GameObject[] resourceCanDrop;
    public int[] resourceCanDropChanceRange;
    public int health;
}
