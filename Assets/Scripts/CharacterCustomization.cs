using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCustomization : Singleton<CharacterCustomization>
{
	public GameObject target;
	public string suffixMax = " Max", suffixMin = " Min";

	private CharacterCustomization (){	}

	private SkinnedMeshRenderer smr;
	private Mesh mesh;

	private Dictionary<string, BlendShape> blendShapeDatabase = new Dictionary<string, BlendShape>();

	private void Start()
	{
		Initialize ();
	}

	#region Public Functions

	public void ChangeBlendShapeValue(string blendShapeName, float value)
	{
		if (!blendShapeDatabase.ContainsKey (blendShapeName))
		{
			Debug.LogError ("BlendShape " + blendShapeName + " doesn't exist.");
			return;
		}

		value = Mathf.Clamp (value, -100.0f, 100.0f);

		BlendShape blendShape = blendShapeDatabase [blendShapeName];

		if (value >= 0) {
			if (blendShape.positiveIndex == -1)
				return;
			
			smr.SetBlendShapeWeight (blendShape.positiveIndex, value);

			if (blendShape.negativeIndex == -1)
				return;

			smr.SetBlendShapeWeight (blendShape.negativeIndex, 0);
		}
		else
		{
			if (blendShape.negativeIndex == -1)
				return;

			smr.SetBlendShapeWeight (blendShape.negativeIndex, -value);

			if (blendShape.positiveIndex == -1)
				return;

			smr.SetBlendShapeWeight (blendShape.positiveIndex, 0);
		}
	}

	#endregion

	#region Private Functions

	private void Initialize()
	{
		smr = GetSkinnedMeshRenderer ();
		mesh = smr.sharedMesh;

		ParseBlendShapesToDictionary ();
	}

	private SkinnedMeshRenderer GetSkinnedMeshRenderer()
	{
		SkinnedMeshRenderer _smr = target.GetComponentInChildren<SkinnedMeshRenderer> ();

		return _smr;
	}

	private void ParseBlendShapesToDictionary()
	{
		List<string> blendShapeNames = Enumerable.Range (0, mesh.blendShapeCount).Select (x => mesh.GetBlendShapeName (x)).ToList();

		for (int i = 0; i < blendShapeNames.Count;)
		{
			string altSuffix, noSuffix;

			if(blendShapeNames[i].Contains(suffixMax))
				noSuffix = blendShapeNames[i].TrimEnd(suffixMax.ToCharArray());
			else
				noSuffix = blendShapeNames[i].TrimEnd(suffixMin.ToCharArray());

			string positiveName = string.Empty, negativeName = string.Empty;

			bool exists = false;

			int positiveIndex = -1, negativeIndex = -1;

			//Positive suffix
			if (blendShapeNames [i].EndsWith (suffixMax)) {
				altSuffix = noSuffix + suffixMin;
				positiveName = blendShapeNames [i];
				negativeName = altSuffix;

				if (blendShapeNames.Contains (altSuffix))
					exists = true;

				positiveIndex = mesh.GetBlendShapeIndex (positiveName);

				if (exists)
					negativeIndex = mesh.GetBlendShapeIndex (negativeName);
			}

			//Negative suffix
			else if (blendShapeNames [i].EndsWith (suffixMin)) {
				altSuffix = noSuffix + suffixMax;
				negativeName = blendShapeNames [i];
				positiveName = altSuffix;

				if (blendShapeNames.Contains (altSuffix))
					exists = true;

				negativeIndex = mesh.GetBlendShapeIndex (negativeName);

				if (exists)
					positiveIndex = mesh.GetBlendShapeIndex (positiveName);
			}
			else
				positiveIndex = mesh.GetBlendShapeIndex (blendShapeNames[i]);

			blendShapeDatabase.Add (noSuffix, new BlendShape (positiveIndex, negativeIndex));

			//Remove selected indexes from list

			if(positiveName != string.Empty)
				blendShapeNames.Remove (positiveName);
			
			if(negativeName != string.Empty)
				blendShapeNames.Remove (negativeName);
		}
	}

	#endregion
}
