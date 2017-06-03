using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingItemData : MonoBehaviour, IPointerDownHandler
{
	public Item item;
	public GameObject requirementSlot;
	public GameObject requirementItem;
	public List<Item> requirements = new List<Item> ();
	public List<GameObject> slots = new List<GameObject> ();

	private Inventory inventory;

	public void Setup(string playerName)
	{
		inventory = GameObject.Find(playerName).GetComponent<Inventory> ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		inventory.DisplayRequirements (item);
	}
}
