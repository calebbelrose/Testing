using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CircularMenu : MonoBehaviour
{
	public List<MenuButton> buttons = new List<MenuButton>();
	public int menuItems;
	public int currMenuItem;
	public BuildingSystem buildingSystem;

	private Vector2 mousePosition;
	private Vector2 fromVector2M = new Vector2(0.5f, 1.0f);
	private Vector2 centerCircle = new Vector2(0.5f, 0.5f);
	private Vector2 toVector2M;
	private int oldMenuItem;

	// Use this for initialization
	void Start ()
	{
		menuItems = buttons.Count;

		foreach (MenuButton button in buttons)
			button.sceneImage.color = 
				button.normalColour;

		currMenuItem = 0;
		oldMenuItem = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetCurrentMenuItem ();
		if (Input.GetMouseButtonDown (0))
			ButtonAction ();
	}

	public void GetCurrentMenuItem()
	{
		mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
		toVector2M = new Vector2 (mousePosition.x / Screen.width, mousePosition.y / Screen.height);
		float angle = (Mathf.Atan2 (fromVector2M.y - centerCircle.y, fromVector2M.x - centerCircle.x) - Mathf.Atan2 (toVector2M.y - centerCircle.y, toVector2M.x - centerCircle.x)) * Mathf.Rad2Deg;

		if (angle < 0.0f)
			angle += 360.0f;

		currMenuItem = (int)(angle / (360 / menuItems));

		if (currMenuItem != oldMenuItem)
		{
			buttons [oldMenuItem].sceneImage.color = buttons [oldMenuItem].normalColour;
			oldMenuItem = currMenuItem;
			buttons [currMenuItem].sceneImage.color = buttons [currMenuItem].highlightedColour;
		}
	}

	public void ButtonAction()
	{
		buttons [currMenuItem].sceneImage.color = buttons [currMenuItem].pressedColour;
		buildingSystem.ChangeCurrentBuilding (currMenuItem);
		buildingSystem.DisableMenu ();
	}
}

[Serializable]
public class MenuButton
{
	public string name;
	public Image sceneImage;
	public Color normalColour = Color.white;
	public Color highlightedColour = Color.grey;
	public Color pressedColour = Color.gray;
}
