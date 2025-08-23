using UnityEngine;

public class GridCell : MonoBehaviour
{
	[Header("Visual Settings")]
	public Color emptyColor = Color.white;
	public Color validHighlightColor = Color.green;
	public Color invalidHighlightColor = Color.red;

	private SpriteRenderer spriteRenderer;
	private bool isOccupied = false;
	private Color originalColor;
	private Vector2Int gridPosition;

	public bool IsOccupied => isOccupied;
	public Vector2Int GridPosition => gridPosition;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = emptyColor;
		spriteRenderer.color = originalColor;
	}

	public void Initialize( int x , int y )
	{
		gridPosition = new Vector2Int(x , y);
	}

	public void OccupyCell( Color pieceColor )
	{
		isOccupied = true;
		spriteRenderer.color = pieceColor;
		originalColor = pieceColor;

		// Add clearing effect
		StartCoroutine(PulseEffect());
	}

	public void ClearCell()
	{
		isOccupied = false;
		originalColor = emptyColor;
		StartCoroutine(ClearEffect());
	}

	public void HighlightValid()
	{
		if (!isOccupied)
			spriteRenderer.color = validHighlightColor;
	}

	public void HighlightInvalid()
	{
		spriteRenderer.color = invalidHighlightColor;
	}

	public void ClearHighlight()
	{
		spriteRenderer.color = originalColor;
	}

	private System.Collections.IEnumerator PulseEffect()
	{
		float duration = 0.2f;
		Color startColor = spriteRenderer.color;
		Color brightColor = Color.white;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			float progress = t / duration;
			spriteRenderer.color = Color.Lerp(startColor , brightColor , Mathf.Sin(progress * Mathf.PI));
			yield return null;
		}

		spriteRenderer.color = originalColor;
	}

	private System.Collections.IEnumerator ClearEffect()
	{
		float duration = 0.3f;
		Color startColor = spriteRenderer.color;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			float progress = t / duration;
			Color currentColor = Color.Lerp(startColor , emptyColor , progress);
			currentColor.a = Mathf.Lerp(1f , 0.3f , progress);
			spriteRenderer.color = currentColor;
			yield return null;
		}

		spriteRenderer.color = emptyColor;
	}
}