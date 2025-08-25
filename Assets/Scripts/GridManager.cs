using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class GridManager : MonoBehaviour
{
	[Header("Grid Settings")]
	public int gridWidth = 8;
	public int gridHeight = 10;
	public float cellSize = 0.8f;

	[Header("Prefabs")]
	public GameObject cellPrefab;
	public GameObject gridLinePrefab;

	private GridCell[,] grid;
	private Vector2 gridOffset;

	[Header("Positioning")]
	public bool centerInCamera = true;
	public float topUISpace = 100f;      // Pixels reserved for score UI
	public float bottomUISpace = 150f;   // Pixels reserved for pieces
	public Vector2 manualOffset = Vector2.zero; // Manual fine-tuning
	private Camera mainCamera;
	public void Initialize()
	{
		mainCamera = Camera.main;
		OptimizeForMobile();
		CreateGrid();
		CalculateGridOffset();
		CenterGridInCamera();
	}

	private void CenterGridInCamera()
	{
		if (!centerInCamera || mainCamera == null) return;

		// Convert UI space from pixels to world units
		float pixelsPerUnit = 100f; // Adjust based on your sprite settings
		float topSpaceWorld = topUISpace / pixelsPerUnit;
		float bottomSpaceWorld = bottomUISpace / pixelsPerUnit;

		// Calculate available screen space
		float cameraHeight = mainCamera.orthographicSize * 2f;
		float availableHeight = cameraHeight - topSpaceWorld - bottomSpaceWorld;

		// Calculate grid center position
		Vector3 gridCenter = new Vector3(
			( gridWidth - 1 ) * cellSize * 0.5f ,
			( gridHeight - 1 ) * cellSize * 0.5f ,
			0
		);

		// Position grid in available space
		Vector3 targetPosition = new Vector3(
			-gridCenter.x + manualOffset.x ,
			-gridCenter.y + ( bottomSpaceWorld - topSpaceWorld ) * 0.5f + manualOffset.y ,
			0
		);

		transform.position = targetPosition;

		Debug.Log($"Grid positioned at: {targetPosition}");
	}

	private void OptimizeForMobile()
	{
		if (Application.isMobilePlatform)
		{
			// Adjust cell size for mobile screens
			float screenWidth = Screen.width;
			float screenHeight = Screen.height;

			// Calculate optimal cell size
			float maxCellSizeForWidth = ( screenWidth * 0.8f ) / gridWidth / 100f;  // 80% of screen width
			float maxCellSizeForHeight = ( screenHeight * 0.6f ) / gridHeight / 100f;  // 60% of screen height

			cellSize = Mathf.Min(cellSize , maxCellSizeForWidth , maxCellSizeForHeight);
			Debug.Log($"Mobile optimization: Cell size adjusted to {cellSize:F2}");
		}
	}

	private void CreateGrid()
	{
		if (cellPrefab == null)
		{
			Debug.LogError("Cell Prefab is not assigned in GridManager!");
			return;
		}

		grid = new GridCell[gridWidth , gridHeight];

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				// Position cells so they align with grid lines
				Vector3 position = new Vector3(x * cellSize , y * cellSize , 0);
				GameObject cellObj = Instantiate(cellPrefab , position , Quaternion.identity , transform);

				// Make sure cell sprite is exactly cellSize
				SpriteRenderer sr = cellObj.GetComponent<SpriteRenderer>();
				if (sr != null)
				{
					sr.size = new Vector2(cellSize , cellSize);
				}

				GridCell cell = cellObj.GetComponent<GridCell>();
				if (cell != null)
				{
					cell.Initialize(x , y);
					grid[x , y] = cell;
				}
			}
		}
	}

	private void CalculateGridOffset()
	{
		gridOffset = new Vector2(-( gridWidth - 1 ) * cellSize / 2 , -( gridHeight - 1 ) * cellSize / 2);
	}

	public bool CanPlacePiece( GamePiece piece , Vector2Int gridPos )
	{
		Vector2Int[] shape = piece.GetCurrentShape();

		foreach (Vector2Int offset in shape)
		{
			int x = gridPos.x + offset.x;
			int y = gridPos.y + offset.y;

			if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
				return false;

			if (grid[x , y].IsOccupied)
				return false;
		}

		return true;
	}

	public void PlacePiece( GamePiece piece , Vector2Int gridPos )
	{
		Vector2Int[] shape = piece.GetCurrentShape();

		foreach (Vector2Int offset in shape)
		{
			int x = gridPos.x + offset.x;
			int y = gridPos.y + offset.y;
			grid[x , y].OccupyCell(piece.PieceColor);
		}
	}

	public Vector2Int WorldToGrid( Vector3 worldPos )
	{
		Vector3 localPos = transform.InverseTransformPoint(worldPos);
		int x = Mathf.RoundToInt(localPos.x / cellSize);
		int y = Mathf.RoundToInt(localPos.y / cellSize);
		return new Vector2Int(x , y);
	}

	public Vector3 GridToWorld( Vector2Int gridPos )
	{
		Vector3 localPos = new Vector3(gridPos.x * cellSize , gridPos.y * cellSize , 0);
		return transform.TransformPoint(localPos);
	}

	public int CheckAndClearLines()
	{
		List<int> rowsToClear = new List<int>();
		List<int> colsToClear = new List<int>();

		// Check rows
		for (int y = 0; y < gridHeight; y++)
		{
			bool isRowComplete = true;
			for (int x = 0; x < gridWidth; x++)
			{
				if (!grid[x , y].IsOccupied)
				{
					isRowComplete = false;
					break;
				}
			}
			if (isRowComplete)
				rowsToClear.Add(y);
		}

		// Check columns
		for (int x = 0; x < gridWidth; x++)
		{
			bool isColComplete = true;
			for (int y = 0; y < gridHeight; y++)
			{
				if (!grid[x , y].IsOccupied)
				{
					isColComplete = false;
					break;
				}
			}
			if (isColComplete)
				colsToClear.Add(x);
		}

		// Clear lines with visual effect
		foreach (int row in rowsToClear)
		{
			ClearRow(row);
		}

		foreach (int col in colsToClear)
		{
			ClearColumn(col);
		}

		return rowsToClear.Count + colsToClear.Count;
	}

	private void ClearRow( int row )
	{
		for (int x = 0; x < gridWidth; x++)
		{
			grid[x , row].ClearCell();
		}
	}

	private void ClearColumn( int col )
	{
		for (int y = 0; y < gridHeight; y++)
		{
			grid[col , y].ClearCell();
		}
	}

	public void HighlightValidPosition( GamePiece piece , Vector2Int gridPos )
	{
		if (CanPlacePiece(piece , gridPos))
		{
			Vector2Int[] shape = piece.GetCurrentShape();
			foreach (Vector2Int offset in shape)
			{
				int x = gridPos.x + offset.x;
				int y = gridPos.y + offset.y;
				if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
				{
					grid[x , y].HighlightValid();
				}
			}
		}
	}

	public void ClearHighlights()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				grid[x , y].ClearHighlight();
			}
		}
	}
}
