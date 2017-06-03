using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
	public List<Collider> coll = new List<Collider>();
	public ObjectSorts sort;
	public Material green;
	public Material red;
	public bool isBuildable;
	public bool second;
	public PreviewObject childColl;
	public Transform graphics;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 9)
				coll.Add (other);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 9)
			coll.Remove (other);
	}

	void Update()
	{
		if(!second)
			ChangeColour ();
	}

	public void ChangeColour()
	{
		switch (sort)
		{
			case ObjectSorts.foundation:
				if (coll.Count == 0)
					isBuildable = true;
				else
					isBuildable = false;
				break;
			case ObjectSorts.wall:
				if (coll.Count == 0 && childColl.coll.Count > 2)
				{
					int pillars = 0;

					foreach (Collider c in childColl.coll)
						if (c.CompareTag ("Pillar"))
							pillars++;
					
					if (pillars == 2)
						isBuildable = true;
					else
						isBuildable = false;
				}
				else
					isBuildable = false;
				break;
			case ObjectSorts.floor:
				if (coll.Count == 0 && childColl.coll.Count > 3)
				{
					int pillars = 0;

					foreach (Collider c in childColl.coll)
						if (c.CompareTag ("Pillar"))
							pillars++;
					
					if (pillars == 4)
						isBuildable = true;
					else
						isBuildable = false;
				}
				else
					isBuildable = false;
				break;
			case ObjectSorts.staircase:
				if (coll.Count == 0 && childColl.coll.Count > 0)
				{
					int pillars = 0;
					int foundation = 0;

					foreach (Collider c in childColl.coll)
						if (c.CompareTag ("Pillar"))
							pillars++;
						else if (c.CompareTag ("Foundation") && c.gameObject.transform.position.x == gameObject.transform.position.x && c.gameObject.transform.position.z == gameObject.transform.position.z)
							foundation++;

					if (foundation == 1 || pillars == 4)
						isBuildable = true;
					else
						isBuildable = false;
				}
				else
					isBuildable = false;
				break;
			case ObjectSorts.pillar:
				if (coll.Count == 0 && childColl.coll.Count == 1)
				{
					if (childColl.coll[0].CompareTag ("Pillar") || childColl.coll[0].CompareTag ("Foundation"))
						isBuildable = true;
					else
						isBuildable = false;
				}
				else
					isBuildable = false;
				break;
			default:
				if (coll.Count == 0 && childColl.coll.Count > 0)
				{
					int pillars = 0;
					int foundation = 0;
					foreach (Collider c in childColl.coll)
						if (c.CompareTag ("Pillar"))
							pillars++;
						else if (c.CompareTag ("Foundation"))
							foundation++;

					if (pillars <= 4 && foundation == 1)
						isBuildable = true;
					else
						isBuildable = false;
				}
				else
					isBuildable = false;
				break;
		}

		if (isBuildable)
			foreach (Transform child in graphics)
				child.GetComponent<Renderer> ().material = green;
		else
			foreach (Transform child in graphics)
				child.GetComponent<Renderer> ().material = red;
	}
}

public enum ObjectSorts
{
	normal,
	foundation,
	floor,
	wall,
	pillar,
	staircase,
	door
}
