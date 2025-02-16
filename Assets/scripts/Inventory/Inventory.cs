using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<Slot> inventorySlots = new List<Slot>();   // �����е���Ʒ��
    public List<Slot> hotbarSlotsInBag = new List<Slot>(); // �����еĿ������Ʒ��
    public List<Slot> hotbarSlots = new List<Slot>();      // ������е���Ʒ��

    private int maxAmountUp = 27;
    private int maxAmountDown = 9;
    private int curAmountUp = 0;
    private int curAmountDown = 0;

    public GameObject gridUp;
    public GameObject gridDown;
    public GameObject hotbar;

    public Player owner;

    public void InitInventory(Player owner)
    {
        this.owner = owner;

        for (int i = 0; i < maxAmountUp; i++)
        {
            Slot newSlot = Instantiate(Resources.Load<Slot>("Prefab/Inventory/Slot"), gridUp.transform);
            newSlot.gameObject.name = "Slot" + i;
            //newSlot.index = index;
            newSlot.ifHotbar = false;
            newSlot.itemInSlot.gameObject.SetActive(false);
            inventorySlots.Add(newSlot);

            if(GameManager.instance.isCreative)
            {
                Item item = Resources.Load<Item>("Items/" + Item.ItemTypes[(int)BlockType.LAVA]);

                newSlot.itemInSlot.item = item;
                newSlot.itemInSlot.image.sprite = item.icon;
                newSlot.itemInSlot.amount = ItemInBag.maxAmount;
                newSlot.itemInSlot.amountTXT.text = newSlot.itemInSlot.amount.ToString();
                newSlot.itemInSlot.gameObject.SetActive(true);
                curAmountUp++;
            }
        }

        for (int i = 0; i < maxAmountDown; i++)
        {
            Slot newSlot = Instantiate(Resources.Load<Slot>("Prefab/Inventory/Slot"), gridDown.transform);
            newSlot.gameObject.name = "Slot" + i;
            //newSlot.index = index;
            newSlot.ifHotbar = true;
            newSlot.itemInSlot.gameObject.SetActive(false);
            hotbarSlotsInBag.Add(newSlot);

            Slot newHbSlot = Instantiate(Resources.Load<Slot>("Prefab/Inventory/Slot"), hotbar.transform);
            newHbSlot.gameObject.name = "Slot" + i;
            //newHbSlot.index = index;
            newHbSlot.ifHotbar = true;
            newHbSlot.itemInSlot.gameObject.SetActive(false);
            hotbarSlots.Add(newHbSlot);

            newSlot.slotHotbar = newHbSlot;
            newHbSlot.slotHotbar = newSlot;
        }
    }

    public void ChangeState()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            gameObject.SetActive(!gameObject.activeSelf);

            UIManager.instance.UpdateUIState();

            if (gameObject.activeSelf)
            {
                UIManager.instance.EnableCursor();
            }
            else
            {
                UIManager.instance.DisableCursor();
            }
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            UIManager.instance.UpdateUIState();
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddItem(Item item)
    {
        // ���ҿ�����Ƿ�����ͬ����Ʒ
        foreach (Slot slot in hotbarSlotsInBag)
        {
            if (slot.itemInSlot.item == item)
            {
                if (slot.itemInSlot.amount >= ItemInBag.maxAmount)
                {
                    continue;
                }
                slot.itemInSlot.amount++;
                slot.itemInSlot.amountTXT.text = (int.Parse(slot.itemInSlot.amountTXT.text) + 1).ToString();

                // ����ǿ��������Ʒ��ҲҪ��������
                //foreach (Slot hbSlot in hotbarSlots)
                //{
                //    if (hbSlot.index == slot.index)
                //    {
                //        hbSlot.itemInSlot.amount++;
                //        hbSlot.itemInSlot.amountTXT.text = (int.Parse(hbSlot.itemInSlot.amountTXT.text) + 1).ToString();
                //        break;
                //    }
                //}

                slot.slotHotbar.itemInSlot.amount++;
                slot.slotHotbar.itemInSlot.amountTXT.text = (int.Parse(slot.slotHotbar.itemInSlot.amountTXT.text) + 1).ToString();

                owner.RefreshBuildType();
                return;
            }
        }

        // ��������޸���Ʒ���߸���Ʒ�����������ڿ�����ﴴ���µ���Ʒ��
        if (curAmountDown < maxAmountDown)
        {
            foreach (Slot slot in hotbarSlotsInBag)
            {
                if (slot.itemInSlot.item == null)
                {
                    //slot.index = ++index;
                    slot.itemInSlot.item = item;
                    slot.itemInSlot.image.sprite = item.icon;
                    slot.itemInSlot.amount++;
                    slot.itemInSlot.amountTXT.text = slot.itemInSlot.amount.ToString();
                    slot.itemInSlot.gameObject.SetActive(true);

                    foreach (Slot hbSlot in hotbarSlots)
                    {
                        if (hbSlot.itemInSlot.item == null)
                        {
                            //hbSlot.index = slot.index;
                            hbSlot.itemInSlot.item = item;
                            hbSlot.itemInSlot.image.sprite = item.icon;
                            hbSlot.itemInSlot.amount++;
                            hbSlot.itemInSlot.amountTXT.text = hbSlot.itemInSlot.amount.ToString();
                            hbSlot.itemInSlot.gameObject.SetActive(true);
                            break;
                        }
                    }

                    curAmountDown++;

                    owner.RefreshBuildType();
                    return;
                }
            }
        }

        // ������������ڱ����ﴴ���µ���Ʒ��
        if (curAmountUp < maxAmountUp)
        {
            foreach(Slot slot in hotbarSlotsInBag)
            {
                if (slot.itemInSlot.item == null)
                {
                    //slot.index = ++index;
                    slot.itemInSlot.item = item;
                    slot.itemInSlot.image.sprite = item.icon;
                    slot.itemInSlot.amount++;
                    slot.itemInSlot.amountTXT.text = slot.itemInSlot.amount.ToString();
                    slot.itemInSlot.gameObject.SetActive(true);
                    curAmountUp++;

                    owner.RefreshBuildType();
                    return;
                }
            }
        }
    }
}
