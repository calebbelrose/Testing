using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BlendShapeSlider : MonoBehaviour
{
	[Header("Do not include the suffix of the Blend Shape name")]
	public string blendShapeName;
	private Slider slider;

	// Use this for initialization
	void Start ()
	{
		slider = GetComponent<Slider>();

		slider.onValueChanged.AddListener (value => CharacterCustomization.Instance.ChangeBlendShapeValue (blendShapeName, value));
	}
}
