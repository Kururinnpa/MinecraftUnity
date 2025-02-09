using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInBag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;
    public Image image;
    public int amount;
    public TextMeshProUGUI amountTXT;
    public static int maxAmount = 2;
    private Inventory inventory;

    Transform preParent; // 移动的物体初始所在的slot

    private void Start()
    {
        inventory = UIManager.instance.inventory.GetComponent<Inventory>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            preParent = transform.parent;
            //transform.SetParent(preParent.parent);
            transform.position = eventData.position;

            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null && eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.tag == "Slot") // 空slot，交换item
            {
                Transform targetItem = eventData.pointerCurrentRaycast.gameObject.transform.Find("Item");
                Slot targetSlot = targetItem.parent.GetComponentInParent<Slot>();
                Slot moveSlot = transform.parent.GetComponentInParent<Slot>();

                // 如果要清空的是快捷栏，那么快捷栏也要清空
                if (moveSlot.ifHotbar)
                {
                    Slot slotHotbar = moveSlot.slotHotbar;
                    slotHotbar.itemInSlot.ClearItem();
                }

                // 如果要修改的是快捷栏，那么快捷栏也要修改
                if (targetSlot.ifHotbar)
                {
                    Slot slotHotbar = targetSlot.slotHotbar;
                    slotHotbar.itemInSlot.item = item;
                    slotHotbar.itemInSlot.image.sprite = item.icon;
                    slotHotbar.itemInSlot.amount = moveSlot.itemInSlot.amount;
                    slotHotbar.itemInSlot.amountTXT.text = slotHotbar.itemInSlot.amount.ToString();
                    slotHotbar.itemInSlot.gameObject.SetActive(true);
                }

                // 交换两个item
                targetItem.SetParent(preParent);
                targetItem.position = preParent.position;
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;


                targetSlot.itemInSlot = targetSlot.transform.Find("Item").GetComponent<ItemInBag>();
                moveSlot.itemInSlot = moveSlot.transform.Find("Item").GetComponent<ItemInBag>();
            }
            else
            {
                GameObject currentObject = eventData.pointerCurrentRaycast.gameObject; // image或者num
                Transform parentTransform = currentObject.transform.parent; // 目标slot的item
                if (parentTransform != null && parentTransform.GetComponent<ItemInBag>() != null) // 含有item的slot
                {
                    Transform targetItem = parentTransform; // 目标slot的item
                    ItemInBag targetItemInBag = targetItem.GetComponent<ItemInBag>();
                    ItemInBag moveItemInBag = transform.GetComponent<ItemInBag>();
                    Slot targetSlot = targetItem.parent.GetComponentInParent<Slot>();
                    Slot moveSlot = transform.parent.GetComponentInParent<Slot>();

                    if (targetItemInBag.item == moveItemInBag.item) // 同一种物品
                    {
                        if (targetItemInBag.amount + moveItemInBag.amount <= maxAmount) // 合并
                        {                           
                            targetItemInBag.amount += moveItemInBag.amount;
                            targetItemInBag.amountTXT.text = targetItemInBag.amount.ToString();
                            
                            // 如果要修改的是快捷栏，那么快捷栏也要修改
                            if (targetSlot.ifHotbar)
                            {
                                targetSlot.slotHotbar.itemInSlot.amount = targetItemInBag.amount;
                                targetSlot.slotHotbar.itemInSlot.amountTXT.text = targetItemInBag.amount.ToString();
                            }
                            
                            // 如果要清空的是快捷栏，那么快捷栏也要清空
                            if (moveSlot.ifHotbar)
                            {
                                moveSlot.slotHotbar.itemInSlot.ClearItem();
                            }

                            // 清空item
                            moveItemInBag.ClearItem();
                        }
                        else // 超过最大容量，优先填满一个slot
                        {
                            moveItemInBag.amount = targetItemInBag.amount + moveItemInBag.amount - maxAmount;
                            moveItemInBag.amountTXT.text = moveItemInBag.amount.ToString();

                            // 如果要修改的是快捷栏，那么快捷栏也要修改
                            if (moveSlot.ifHotbar)
                            {
                                moveSlot.slotHotbar.itemInSlot.amount = moveItemInBag.amount;
                                moveSlot.slotHotbar.itemInSlot.amountTXT.text = moveItemInBag.amount.ToString();
                            }

                            targetItemInBag.amount = maxAmount;
                            targetItemInBag.amountTXT.text = targetItemInBag.amount.ToString();

                             // 如果要修改的是快捷栏，那么快捷栏也要修改
                            if (targetSlot.ifHotbar)
                            {
                                targetSlot.slotHotbar.itemInSlot.amount = targetItemInBag.amount;
                                targetSlot.slotHotbar.itemInSlot.amountTXT.text = targetItemInBag.amount.ToString();
                            }

                            transform.SetParent(preParent);
                            transform.position = preParent.position;
                        }
                    }
                    else // 不同种物品
                    {
                        if (targetSlot.ifHotbar) // 终点是快捷栏
                        {
                            Slot slotHotbar = targetSlot.slotHotbar;
                            slotHotbar.itemInSlot.item = item;
                            slotHotbar.itemInSlot.image.sprite = item.icon;
                            slotHotbar.itemInSlot.amount = moveSlot.itemInSlot.amount;
                            slotHotbar.itemInSlot.amountTXT.text = slotHotbar.itemInSlot.amount.ToString();
                        }

                        if (moveSlot.ifHotbar) // 起点是快捷栏
                        {
                            Slot slotHotbar = moveSlot.slotHotbar;
                            slotHotbar.itemInSlot.item = targetSlot.itemInSlot.item;
                            slotHotbar.itemInSlot.image.sprite = targetSlot.itemInSlot.item.icon;
                            slotHotbar.itemInSlot.amount = targetSlot.itemInSlot.amount;
                            slotHotbar.itemInSlot.amountTXT.text = slotHotbar.itemInSlot.amount.ToString();
                        }

                        targetItem.SetParent(preParent);
                        targetItem.position = preParent.position;
                        transform.SetParent(targetSlot.transform);
                        transform.position = targetSlot.transform.position;

                        targetSlot.itemInSlot =targetSlot.transform.Find("Item").GetComponent<ItemInBag>();
                        moveSlot.itemInSlot = moveSlot.transform.Find("Item").GetComponent<ItemInBag>();
                    }
                }
                else // 不是slot，返回原位
                {
                    transform.SetParent(preParent);
                    transform.position = preParent.position;
                }
            }

            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            inventory.owner.RefreshBuildType();
        }
        else if(eventData.pointerCurrentRaycast.gameObject == null) // 拖到背包外面
        {
            transform.SetParent(preParent);
            transform.position = preParent.position;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            inventory.owner.RefreshBuildType();
        }
    }

    public void ClearItem()
    {
        item = null;
        image.sprite = null;
        amount = 0;
        amountTXT.text = "";
        gameObject.SetActive(false);
    }
}
