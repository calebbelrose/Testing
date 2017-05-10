using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
	public int id;

	private Inventory inventory;

	void Start()
	{
		inventory = GameObject.Find ("Inventory").GetComponent<Inventory> ();
	}

	public void OnDrop (PointerEventData eventData)
	{
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData> ();

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
			ItemData itemData = item.GetComponent<ItemData> ();
			itemData.slot = droppedItem.slot;
			item.transform.SetParent(inventory.slots[droppedItem.slot].transform);
			item.transform.position = inventory.slots [droppedItem.slot].transform.position;

			droppedItem.transform.SetParent(transform);
			droppedItem.transform.position = transform.position;

			inventory.items [droppedItem.slot] = itemData.item;
			inventory.items [id] = droppedItem.item;
			droppedItem.slot = id;
		}
	}
}
