using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public GameObject inventorySlot;
	public GameObject inventoryItem;
	public GameObject inventoryPanel;
	public GameObject slotPanel;
	public List<Item> items = new List<Item> ();
	public List<GameObject> slots = new List<GameObject> ();

	private ItemDatabase database;
	private int slotAmount;

	void Start()
	{
		database = GetComponent<ItemDatabase> ();
		slotAmount = 54;
		for (int currSlot = 0; currSlot < slotAmount; currSlot++)
		{
			items.Add (new Item ());
			slots.Add (Instantiate (inventorySlot));
			slots [currSlot].GetComponent<Slot> ().id = currSlot;
			slots [currSlot].transform.SetParent (slotPanel.transform);
		}

		Additem (0);
		Additem (2);
		Additem (2);
		Additem (1);
		Additem (2);
	}

	public void Additem(int id)
	{
		Item itemToAdd = database.FetchItemByID (id);

		if (itemToAdd.stackable)
		{
			int slot = GetSlot (itemToAdd);

			if (slot != -1)
				AddToStack (slot);
			else
			{
				slot = AddNewItem (itemToAdd);
				AddToStack (slot);
			}
		}
		else
			AddNewItem (itemToAdd);
	}

	int GetSlot(Item item)
	{
		for (int currItem = 0; currItem < items.Count; currItem++)
			if (items [currItem].id == item.id)
				return currItem;
		
		return -1;
	}

	int AddNewItem(Item itemToAdd)
	{
		for (int currItem = 0; currItem < items.Count; currItem++) {
			if (items [currItem].id == -1) {
				items [currItem] = itemToAdd;
				GameObject itemObj = Instantiate (inventoryItem);
				ItemData itemData = itemObj.GetComponent<ItemData> ();
				itemData.item = itemToAdd;
				itemData.slot = currItem;
				itemObj.transform.SetParent (slots [currItem].transform);
				itemObj.GetComponent<Image> ().sprite = itemToAdd.sprite;
				itemObj.transform.position = Vector2.zero;
				itemObj.name = itemToAdd.title;
				return currItem;
			}
		}

		return -1;
	}

	void AddToStack(int slot)
	{
		ItemData data = slots [slot].transform.GetChild (0).GetComponent<ItemData> ();
		data.amount++;
		data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
	}

	public void RemoveItem(int id)
	{
		Item itemToRemove = database.FetchItemByID (id);
		int slot = GetSlot (itemToRemove);

		if(slot != -1)
		{
			ItemData itemData = slots [slot].transform.GetChild (0).GetComponent<ItemData> ();

			if (itemToRemove.stackable && itemData.amount > 1)
			{
				itemData.amount--;
				itemData.transform.GetChild (0).GetComponent<Text> ().text = itemData.amount.ToString ();
			}
			else
				DestroyItem (slot);
		}
	}

	void DestroyItem(int slot)
	{
		Destroy (slots [slot].transform.GetChild (0).gameObject);
		items [slot] = new Item ();
	}
}
