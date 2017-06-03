using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemData : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Item item;
	public int amount;
	public int slot;

	private Vector2 offset;
	private Inventory inventory;
	private Tooltip tooltip;
	private int previousSlot;

	public void Setup(string playerName)
	{
		GameObject player = GameObject.Find (playerName);
		inventory = player.GetComponent<Inventory>();
		tooltip = player.GetComponent<Tooltip> ();
	}

	void MoveToSlot(int slot)
	{
		transform.SetParent (inventory.inventorySlots [slot].transform);
		transform.position = inventory.inventorySlots [slot].transform.position;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (item != null)
		{
			offset = new Vector2 (eventData.position.x - transform.position.x, eventData.position.y - transform.position.y);
			transform.SetParent (transform.parent.parent);
			transform.position = eventData.position - offset;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			previousSlot = slot;
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		if (item != null)
			transform.position = eventData.position;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if (slot == 53)
		{
			slot = previousSlot;
			MoveToSlot (slot);
			inventory.RemoveItem (item.id, amount);
		}
		else
			MoveToSlot (slot);
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		tooltip.Activate (item);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		tooltip.Deactivate ();
	}
}
