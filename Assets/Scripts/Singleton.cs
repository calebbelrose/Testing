using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
				instance = new GameObject ().AddComponent<T> ();

			return instance;
		}
	}

	public void Awake()
	{
		if (instance != null)
			Debug.Log ("Other instance of " + this.GetType ().Name + " has been destroyed!");

		instance = this as T;
	}
}
