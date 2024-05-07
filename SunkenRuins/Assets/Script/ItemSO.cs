using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    HealthPotion,
    PowerBattery,
    BubbleShield,
}

[CreateAssetMenu(menuName = "ScriptableObjects/ItemScriptableObject")]
public class ItemSO : ScriptableObject
{

    public Transform prefab;
    public ItemType itemType;
    public Sprite sprite;
    public string itemName;

}
