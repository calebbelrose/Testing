using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public GameObject toolTip;

	private Item item;
	private string data;

	void Update()
	{
		if (toolTip.activeSelf)
			toolTip.transform.position = Input.mousePosition;
	}

	public void Activate(Item itemToActivate)
	{
		item = itemToActivate;
		ConstructDataString ();
		toolTip.SetActive (true);
	}

	public void Deactivate()
	{
		toolTip.SetActive (false);
	}

	public void ConstructDataString()
	{
		data = "<color=#FF0000><b>" + item.title + "</b></color>\n\n<color=#FFFFFF>" + item.description + "</color>";
		toolTip.transform.GetChild (0).GetComponent<Text> ().text = data;
	}
}
