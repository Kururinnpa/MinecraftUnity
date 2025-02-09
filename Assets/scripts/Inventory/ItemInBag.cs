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

    Transform preParent; // �ƶ��������ʼ���ڵ�slot

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
            if (eventData.pointerCurrentRaycast.gameObject.tag == "Slot") // ��slot������item
            {
                Transform targetItem = eventData.pointerCurrentRaycast.gameObject.transform.Find("Item");
                Slot targetSlot = targetItem.parent.GetComponentInParent<Slot>();
                Slot moveSlot = transform.parent.GetComponentInParent<Slot>();

                // ���Ҫ��յ��ǿ��������ô�����ҲҪ���
                if (moveSlot.ifHotbar)
                {
                    Slot slotHotbar = moveSlot.slotHotbar;
                    slotHotbar.itemInSlot.ClearItem();
                }

                // ���Ҫ�޸ĵ��ǿ��������ô�����ҲҪ�޸�
                if (targetSlot.ifHotbar)
                {
                    Slot slotHotbar = targetSlot.slotHotbar;
                    slotHotbar.itemInSlot.item = item;
                    slotHotbar.itemInSlot.image.sprite = item.icon;
                    slotHotbar.itemInSlot.amount = moveSlot.itemInSlot.amount;
                    slotHotbar.itemInSlot.amountTXT.text = slotHotbar.itemInSlot.amount.ToString();
                    slotHotbar.itemInSlot.gameObject.SetActive(true);
                }

                // ��������item
                targetItem.SetParent(preParent);
                targetItem.position = preParent.position;
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;


                targetSlot.itemInSlot = targetSlot.transform.Find("Item").GetComponent<ItemInBag>();
                moveSlot.itemInSlot = moveSlot.transform.Find("Item").GetComponent<ItemInBag>();
            }
            else
            {
                GameObject currentObject = eventData.pointerCurrentRaycast.gameObject; // image����num
                Transform parentTransform = currentObject.transform.parent; // Ŀ��slot��item
                if (parentTransform != null && parentTransform.GetComponent<ItemInBag>() != null) // ����item��slot
                {
                    Transform targetItem = parentTransform; // Ŀ��slot��item
                    ItemInBag targetItemInBag = targetItem.GetComponent<ItemInBag>();
                    ItemInBag moveItemInBag = transform.GetComponent<ItemInBag>();
                    Slot targetSlot = targetItem.parent.GetComponentInParent<Slot>();
                    Slot moveSlot = transform.parent.GetComponentInParent<Slot>();

                    if (targetItemInBag.item == moveItemInBag.item) // ͬһ����Ʒ
                    {
                        if (targetItemInBag.amount + moveItemInBag.amount <= maxAmount) // �ϲ�
                        {                           
                            targetItemInBag.amount += moveItemInBag.amount;
                            targetItemInBag.amountTXT.text = targetItemInBag.amount.ToString();
                            
                            // ���Ҫ�޸ĵ��ǿ��������ô�����ҲҪ�޸�
                            if (targetSlot.ifHotbar)
                            {
                                targetSlot.slotHotbar.itemInSlot.amount = targetItemInBag.amount;
                                targetSlot.slotHotbar.itemInSlot.amountTXT.text = targetItemInBag.amount.ToString();
                            }
                            
                            // ���Ҫ��յ��ǿ��������ô�����ҲҪ���
                            if (moveSlot.ifHotbar)
                            {
                                moveSlot.slotHotbar.itemInSlot.ClearItem();
                            }

                            // ���item
                            moveItemInBag.ClearItem();
                        }
                        else // ���������������������һ��slot
                        {
                            moveItemInBag.amount = targetItemInBag.amount + moveItemInBag.amount - maxAmount;
                            moveItemInBag.amountTXT.text = moveItemInBag.amount.ToString();

                            // ���Ҫ�޸ĵ��ǿ��������ô�����ҲҪ�޸�
                            if (moveSlot.ifHotbar)
                            {
                                moveSlot.slotHotbar.itemInSlot.amount = moveItemInBag.amount;
                                moveSlot.slotHotbar.itemInSlot.amountTXT.text = moveItemInBag.amount.ToString();
                            }

                            targetItemInBag.amount = maxAmount;
                            targetItemInBag.amountTXT.text = targetItemInBag.amount.ToString();

                             // ���Ҫ�޸ĵ��ǿ��������ô�����ҲҪ�޸�
                            if (targetSlot.ifHotbar)
                            {
                                targetSlot.slotHotbar.itemInSlot.amount = targetItemInBag.amount;
                                targetSlot.slotHotbar.itemInSlot.amountTXT.text = targetItemInBag.amount.ToString();
                            }

                            transform.SetParent(preParent);
                            transform.position = preParent.position;
                        }
                    }
                    else // ��ͬ����Ʒ
                    {
                        if (targetSlot.ifHotbar) // �յ��ǿ����
                        {
                            Slot slotHotbar = targetSlot.slotHotbar;
                            slotHotbar.itemInSlot.item = item;
                            slotHotbar.itemInSlot.image.sprite = item.icon;
                            slotHotbar.itemInSlot.amount = moveSlot.itemInSlot.amount;
                            slotHotbar.itemInSlot.amountTXT.text = slotHotbar.itemInSlot.amount.ToString();
                        }

                        if (moveSlot.ifHotbar) // ����ǿ����
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
                else // ����slot������ԭλ
                {
                    transform.SetParent(preParent);
                    transform.position = preParent.position;
                }
            }

            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            inventory.owner.RefreshBuildType();
        }
        else if(eventData.pointerCurrentRaycast.gameObject == null) // �ϵ���������
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
