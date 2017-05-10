// Class for positive and negative values of Blend Shapes
public class BlendShape
{
	public int positiveIndex { get; set; }
	public int negativeIndex{ get; set; }

	public BlendShape(int positiveIndex, int negativeIndex)
	{
		this.positiveIndex = positiveIndex;
		this.negativeIndex = negativeIndex;
	}
}
