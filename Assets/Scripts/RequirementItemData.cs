using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RequirementItemData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Item item;
	public int amount;

	private Tooltip tooltip;

	public void Setup(string playerName)
	{
		tooltip = GameObject.Find (playerName).GetComponent<Tooltip> ();
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
