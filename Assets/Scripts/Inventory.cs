using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Inventory : NetworkBehaviour
{
	public GameObject slot;
	public GameObject inventoryItem;
	public GameObject inventoryPanel;
	public GameObject inventorySlotPanel;
	public List<Item> items = new List<Item> ();
	public List<GameObject> inventorySlots = new List<GameObject> ();
	public GameObject requirementItem;
	public GameObject requirementSlotPanel;
	public List<Item> requirements = new List<Item> ();
	public List<GameObject> requirementSlots = new List<GameObject> ();

	private ItemDatabase database;
	private int slotAmount;
	private Item itemToCraft;
	private bool requirementsMet;
	private string playerName;

	void Start()
	{
		if (isLocalPlayer)
		{
			database = GameObject.Find ("Item Database").GetComponent<ItemDatabase> ();
			playerName = gameObject.GetComponent<FollowCamera> ().playerName;

			slotAmount = 54;
			for (int currSlot = 0; currSlot < slotAmount; currSlot++) {
				items.Add (new Item ());
				inventorySlots.Add (Instantiate (slot));
				Slot thisSlot = inventorySlots [currSlot].GetComponent<Slot> ();
				thisSlot.Setup (playerName);
				thisSlot.id = currSlot;
				inventorySlots [currSlot].transform.SetParent (inventorySlotPanel.transform);
			}

			AddItem (0);
			AddItem (2);
			AddItem (2);
			AddItem (1);
			AddItem (2);
			AddItem (3);
			AddItem (3);
			AddItem (3);
			AddItem (3);
			AddItem (3);

			DisplayRequirements (items [0]);
		}
	}

	public void AddItem(int id)
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
		for (int currItem = 0; currItem < items.Count; currItem++)
		{
			if (items [currItem].id == -1)
			{
				items [currItem] = itemToAdd;
				GameObject itemObj = Instantiate (inventoryItem);
				InventoryItemData itemData = itemObj.GetComponent<InventoryItemData> ();
				itemData.Setup (playerName);
				itemData.item = itemToAdd;
				itemData.slot = currItem;
				itemObj.transform.SetParent (inventorySlots [currItem].transform);
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
		InventoryItemData data = inventorySlots [slot].transform.GetChild (0).GetComponent<InventoryItemData> ();
		data.amount++;
		data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
	}

	public void RemoveItem(int id, int amount)
	{
		Item itemToRemove = database.FetchItemByID (id);
		int slot = GetSlot(itemToRemove);
		InventoryItemData itemData = inventorySlots [slot].transform.GetChild (0).GetComponent<InventoryItemData> ();

		if (itemToRemove.stackable && itemData.amount > 1)
		{
			itemData.amount -= amount;
			if (itemData.amount == 0)
				DestroyItem (slot);
			else
				itemData.transform.GetChild (0).GetComponent<Text> ().text = itemData.amount.ToString ();
		}
		else
		{
			int amountRemoved = 0;
			do
			{
				DestroyItem (GetSlot (itemToRemove));
				amountRemoved++;
			}while(amountRemoved < amount);
		}
	}

	void DestroyItem(int slot)
	{
		Destroy (inventorySlots [slot].transform.GetChild (0).gameObject);
		items [slot] = new Item ();
	}

	public int GetCountOfItem (Item item)
	{
		if (item.stackable)
		{
			int slot = GetSlot (item);
			if (slot != -1)
				return inventorySlots [slot].transform.GetChild (0).GetComponent<InventoryItemData> ().amount;
			else
				return 0;
		}
		else
		{
			int count = 0;
			for (int currentItem = 0; currentItem < items.Count; currentItem++)
				if (items [currentItem] == item)
					count++;
			return count;
		}
	}

	public void DisplayRequirements(Item item)
	{
		if (item != null)
		{
			requirementsMet = true;
			itemToCraft = item;
			requirements.Clear ();
			requirementSlots.Clear ();
			foreach (Transform child in requirementSlotPanel.transform)
				GameObject.Destroy (child.gameObject);

			for (int requirementNum = 0; requirementNum < item.requirements.Count; requirementNum++)
			{
				requirements.Add (new Item ());
				requirementSlots.Add (Instantiate (slot));
				Slot thisSlot = requirementSlots [requirementNum].GetComponent<Slot> ();
				thisSlot.Setup (playerName);
				thisSlot.id = requirementNum;
				requirementSlots [requirementNum].transform.SetParent (requirementSlotPanel.transform);
				AddNewRequirement (database.FetchItemBySLUG (item.requirements [requirementNum].slug), item.requirements [requirementNum].amount);
			}
		}
	}

	int AddNewRequirement(Item itemToAdd, int amount)
	{
		for (int currItem = 0; currItem < requirements.Count; currItem++)
		{
			if (requirements [currItem].id == -1)
			{
				int count = GetCountOfItem (itemToAdd);
				requirements [currItem] = itemToAdd;
				GameObject itemObj = Instantiate (requirementItem);
				RequirementItemData itemData = itemObj.GetComponent<RequirementItemData> ();
				Text text = itemData.transform.GetChild (0).GetComponent<Text> ();
				itemData.Setup(playerName);
				itemData.item = itemToAdd;
				itemData.amount = amount;
				text.text = count.ToString() + "/" + itemData.amount.ToString ();
				if (count < itemData.amount)
				{
					text.color = Color.red;
					requirementsMet = false;
				}
				itemObj.transform.SetParent (requirementSlots [currItem].transform);
				itemObj.GetComponent<Image> ().sprite = itemToAdd.sprite;
				itemObj.transform.position = Vector2.zero;
				itemObj.name = itemToAdd.title;
				return currItem;
			}
		}
		return -1;
	}

	public void CraftItem()
	{
		if (requirementsMet)
		{
			for (int requirementNum = 0; requirementNum < itemToCraft.requirements.Count; requirementNum++)
				RemoveItem (database.FetchItemBySLUG(itemToCraft.requirements[requirementNum].slug).id, itemToCraft.requirements[requirementNum].amount);
			
			AddItem (itemToCraft.id);
			DisplayRequirements (itemToCraft);
		}
	}
}
