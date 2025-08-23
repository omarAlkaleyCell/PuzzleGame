using UnityEngine;

public class PieceBlock : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void SetColor( Color color )
	{
		spriteRenderer.color = color;
	}

	public void SetSortingOrder( int order )
	{
		spriteRenderer.sortingOrder = order;
	}
}