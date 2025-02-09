using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    //public int index;
    public bool ifHotbar;
    public ItemInBag itemInSlot;
    public Slot slotHotbar;
    public Image selection;

    public void BuildOneBlock()
    {
        if (!ifHotbar)
            return;

        if (itemInSlot.item != null)
        {
            itemInSlot.amount--; // ÊýÁ¿¼õÒ»

            if (itemInSlot.amount == 0)
            {
                slotHotbar.itemInSlot.ClearItem();
                itemInSlot.ClearItem();
            }
            else
            {
                slotHotbar.itemInSlot.amount--;
                slotHotbar.itemInSlot.amountTXT.text = slotHotbar.itemInSlot.amount.ToString();
                itemInSlot.amountTXT.text = itemInSlot.amount.ToString();
            }
        }
    }
}