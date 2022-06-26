using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Crafting : MonoBehaviour
{
	public GameObject slot;
	public GameObject craftingItem;
	public GameObject craftingPanel;
	public GameObject craftingSlotPanel;
	public List<Item> recipes = new List<Item> ();
	public List<GameObject> slots = new List<GameObject> ();

	private ItemDatabase database;
	private string playerName;

	void Start()
	{
		//if (isLocalPlayer)
		{
			database = GameObject.Find ("Item Database").GetComponent<ItemDatabase> ();
			playerName = gameObject.GetComponent<FollowCamera> ().playerName;

			for (int currSlot = 0; currSlot < database.numItems; currSlot++)
			{
				Item item = database.FetchItemByID (currSlot);
				if (item.requirements.Count > 0)
				{
					recipes.Add (new Item ());
					slots.Add (Instantiate (slot));
					slots [currSlot].GetComponent<Slot> ().Setup (playerName);
					slots [currSlot].GetComponent<Slot> ().id = currSlot;
					slots [currSlot].transform.SetParent (craftingSlotPanel.transform);
					AddItem (item.id);
				}
			}
		}
	}

	public void AddItem(int id)
	{
		AddNewRecipe (database.FetchItemByID (id));
	}

	int GetSlot(Item item)
	{
		for (int currItem = 0; currItem < recipes.Count; currItem++)
			if (recipes [currItem].id == item.id)
				return currItem;

		return -1;
	}

	int AddNewRecipe(Item itemToAdd)
	{
		for (int currItem = 0; currItem < recipes.Count; currItem++) {
			if (recipes [currItem].id == -1) {
				recipes [currItem] = itemToAdd;
				GameObject itemObj = Instantiate (craftingItem);
				CraftingItemData itemData = itemObj.GetComponent<CraftingItemData> ();
				itemData.Setup (playerName);
				itemData.item = itemToAdd;
				itemObj.transform.SetParent (slots [currItem].transform);
				itemObj.GetComponent<Image> ().sprite = itemToAdd.sprite;
				itemObj.transform.position = Vector2.zero;
				itemObj.name = itemToAdd.title;
				return currItem;
			}
		}

		return -1;
	}
}
