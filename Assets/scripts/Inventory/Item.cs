using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public BlockType bType;
    public static string[] ItemTypes = { "", "Grass", "Dirt", "Stone", "Sand", "CobbleStone", "CoalOre", "IronOre", "GoldOre", "DiamondOre" };
}
