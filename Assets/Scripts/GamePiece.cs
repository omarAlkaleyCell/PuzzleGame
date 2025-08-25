using UnityEngine;

public class GamePiece : MonoBehaviour
{
	[Header("Piece Settings")]
	public PieceType pieceType;
	public Color pieceColor;

	[Header("Drag Settings")]
	public float dragSpeed = 10f;
	public LayerMask gridLayerMask = 1;

	private Vector3 originalPosition;
	private bool isDragging = false;
	private Camera mainCamera;
	private GridManager gridManager;
	private PieceBlock[] blocks;
	private Vector2Int[] currentShape;

	public Color PieceColor => pieceColor;
	public bool IsDragging => isDragging;

	private void Awake()
	{
		mainCamera = Camera.main;
		gridManager = FindFirstObjectByType<GridManager>();
		blocks = GetComponentsInChildren<PieceBlock>();
		originalPosition = transform.position;
	}

	public void SetupPiece(PieceType pieceType, Color pieceColor)
	{
		currentShape = PieceShapes.GetShape(pieceType);

		// Position and activate only the blocks we need
		for (int i = 0; i < blocks.Length; i++)
		{
			if (i < currentShape.Length)
			{
				// Activate and position the block
				blocks[i].gameObject.SetActive(true);
				blocks[i].transform.localPosition = new Vector3(
					currentShape[i].x * 0.6f ,
					currentShape[i].y * 0.6f ,
					0
				);
				blocks[i].SetColor(pieceColor);
			}
			else
			{
				// Disable extra blocks
				blocks[i].gameObject.SetActive(false);
			}
		}
	}

	private void OnMouseDown()
	{
		if (!isDragging)
		{
			StartDrag();
		}
	}

	private void OnMouseDrag()
	{
		if (isDragging)
		{
			Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = transform.position.z;
			transform.position = Vector3.Lerp(transform.position , mousePos , dragSpeed * Time.deltaTime);

			// Show placement preview
			Vector2Int gridPos = gridManager.WorldToGrid(transform.position);
			gridManager.ClearHighlights();
			gridManager.HighlightValidPosition(this , gridPos);
		}
	}

	private void OnMouseUp()
	{
		if (isDragging)
		{
			StopDrag();
		}
	}

	private void StartDrag()
	{
		isDragging = true;
		transform.localScale = Vector3.one * 1.1f; // Slight scale increase when dragging
		SetSortingOrder(10); // Bring to front
	}

	private void StopDrag()
	{
		isDragging = false;
		transform.localScale = Vector3.one;
		SetSortingOrder(0);
		gridManager.ClearHighlights();

		Vector2Int gridPos = gridManager.WorldToGrid(transform.position);

		if (gridManager.CanPlacePiece(this , gridPos))
		{
			PlacePiece(gridPos);
		}
		else
		{
			ReturnToOriginalPosition();
		}
	}

	private void PlacePiece( Vector2Int gridPos )
	{
		gridManager.PlacePiece(this , gridPos);
		Vector3 worldPos = gridManager.GridToWorld(gridPos);
		transform.position = worldPos;

		// Disable interaction
		GetComponent<Collider2D>().enabled = false;

		// Notify game manager
		GameManager.Instance.OnPiecePlaced(this);

		// Destroy the piece object after a short delay
		Destroy(gameObject , 0.1f);
	}

	private void ReturnToOriginalPosition()
	{
		StartCoroutine(ReturnToPositionCoroutine());
	}

	private System.Collections.IEnumerator ReturnToPositionCoroutine()
	{
		float duration = 0.3f;
		Vector3 startPos = transform.position;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			float progress = t / duration;
			transform.position = Vector3.Lerp(startPos , originalPosition , progress);
			yield return null;
		}

		transform.position = originalPosition;
	}

	private void SetSortingOrder( int order )
	{
		foreach (PieceBlock block in blocks)
		{
			block.SetSortingOrder(order);
		}
	}

	public Vector2Int[] GetCurrentShape()
	{
		return currentShape;
	}

	public bool CanFitOnGrid()
	{
		// Check if this piece can fit anywhere on the grid
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				if (gridManager.CanPlacePiece(this , new Vector2Int(x , y)))
				{
					return true;
				}
			}
		}
		return false;
	}
}