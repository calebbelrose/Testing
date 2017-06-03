using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
	public int id;

	private Inventory inventory;
	private string playerName;

	public void Setup(string name)
	{
		playerName = name;
		inventory = GameObject.Find (playerName).GetComponent<Inventory> ();
	}

	public void OnDrop (PointerEventData eventData)
	{
		InventoryItemData droppedItem = eventData.pointerDrag.GetComponent<InventoryItemData> ();

		if (inventory.items [id].id == -1)
		{
			if (id != 53)
			{
				inventory.items [droppedItem.slot] = new Item ();
				inventory.items [id] = droppedItem.item;
			}
			droppedItem.slot = id;
		}
		else
		{
			Transform item = transform.GetChild(0);
			InventoryItemData itemData = item.GetComponent<InventoryItemData> ();
			itemData.slot = droppedItem.slot;
			item.transform.SetParent(inventory.inventorySlots[droppedItem.slot].transform);
			item.transform.position = inventory.inventorySlots [droppedItem.slot].transform.position;

			droppedItem.transform.SetParent(transform);
			droppedItem.transform.position = transform.position;

			inventory.items [droppedItem.slot] = itemData.item;
			inventory.items [id] = droppedItem.item;
			droppedItem.slot = id;
		}
	}
}
