using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class ItemDatabase : MonoBehaviour
{
	private List<Item> database = new List<Item>();
	private JsonData itemData;
	public int numItems;

	void Start()
	{
		itemData = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/StreamingAssets/Items.json"));
		ConstructItemDatabase ();
	}

	public Item FetchItemByID(int id)
	{
		for (int currentItem = 0; currentItem < database.Count; currentItem++)
			if (database [currentItem].id == id)
				return database [currentItem];

		return null;
	}

	void ConstructItemDatabase()
	{
		for (int currentItem = 0; currentItem < itemData.Count; currentItem++)
		{
			database.Add(new Item((int)itemData[currentItem]["id"], itemData[currentItem]["title"].ToString(),
				(int)itemData[currentItem]["value"], (int)itemData[currentItem]["stats"]["power"],
				(int)itemData[currentItem]["stats"]["defence"], (int)itemData[currentItem]["stats"]["vitality"],
				itemData[currentItem]["description"].ToString(), (bool)itemData[currentItem]["stackable"],
				(int)itemData[currentItem]["rarity"], itemData[currentItem]["slug"].ToString(), itemData[currentItem]["craftingrequirements"]));
		}
		numItems = itemData.Count;
	}
}

public class Item
{
	public int id { get; set; }
	public string title { get; set; }
	public int value { get; set; }
	public int power { get; set; }
	public int defence { get; set; }
	public int vitality { get; set; }
	public string description { get; set; }
	public bool stackable { get; set; }
	public int rarity { get; set; }
	public string slug { get; set; }
	public List<Requirement> requirements { get; set; }
	public Sprite sprite { get; set; }

	public Item(int newID, string newTitle, int newValue, int newPower, int newDefence, int newVitality, string newDescription, bool newStackable, int newRarity, string newSlug, object newCraftingRequirements)
	{
		id = newID;
		title = newTitle;
		value = newValue;
		power = newPower;
		defence = newDefence;
		vitality = newVitality;
		description = newDescription;
		stackable = newStackable;
		rarity = newRarity;
		slug = newSlug;
		requirements = new List<Requirement> ();
		IEnumerable enumerable = newCraftingRequirements as IEnumerable;
		foreach (object o in enumerable)
		{
			string[] split = o.ToString ().Split (' ');
			requirements.Add (new Requirement (int.Parse(split [0]), split [1]));
		}
		sprite = Resources.Load<Sprite> ("Sprites/" + slug);
	}

	public Item()
	{
		id = -1;
	}
}

public class Requirement
{
	public int amount { get; set; }
	public string slug { get; set; }

	public Requirement(int newAmount, string newSlug)
	{
		amount = newAmount;
		slug = newSlug;
	}
}