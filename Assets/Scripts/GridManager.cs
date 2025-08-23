using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
	[Header("Grid Settings")]
	public int gridWidth = 8;
	public int gridHeight = 10;
	public float cellSize = 0.8f;
	public Color gridLineStartColor = Color.gray;
	public Color gridLineEndColor = Color.white;
	public float gridLineWidth = 0.02f;

	[Header("Prefabs")]
	public GameObject cellPrefab;
	public GameObject gridLinePrefab;

	private GridCell[,] grid;
	private Vector2 gridOffset;

	public void Initialize()
	{
		CreateGrid();
		DrawGridLines();
		CalculateGridOffset();
	}

	private void CreateGrid()
	{
		grid = new GridCell[gridWidth , gridHeight];

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				Vector3 position = new Vector3(x * cellSize , y * cellSize , 0);
				GameObject cellObj = Instantiate(cellPrefab , position , Quaternion.identity , transform);
				GridCell cell = cellObj.GetComponent<GridCell>();
				cell.Initialize(x , y);
				grid[x , y] = cell;
			}
		}
	}

	private void DrawGridLines()
	{
		// Vertical lines
		for (int x = 0; x <= gridWidth; x++)
		{
			Vector3 start = new Vector3(x * cellSize - cellSize / 2 , -cellSize / 2 , -0.1f);
			Vector3 end = new Vector3(x * cellSize - cellSize / 2 , ( gridHeight - 0.5f ) * cellSize , -0.1f);
			CreateGridLine(start , end);
		}

		// Horizontal lines
		for (int y = 0; y <= gridHeight; y++)
		{
			Vector3 start = new Vector3(-cellSize / 2 , y * cellSize - cellSize / 2 , -0.1f);
			Vector3 end = new Vector3(( gridWidth - 0.5f ) * cellSize , y * cellSize - cellSize / 2 , -0.1f);
			CreateGridLine(start , end);
		}
	}

	private void CreateGridLine( Vector3 start , Vector3 end )
	{
		GameObject line = Instantiate(gridLinePrefab , transform);
		LineRenderer lr = line.GetComponent<LineRenderer>();

		// Configure line renderer properties
		lr.positionCount = 2;
		lr.SetPosition(0 , start);
		lr.SetPosition(1 , end);

		// Set gradient colors
		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(gridLineStartColor , 0.0f) , new GradientColorKey(gridLineEndColor , 1.0f) } ,
			new GradientAlphaKey[] { new GradientAlphaKey(1.0f , 0.0f) , new GradientAlphaKey(1.0f , 1.0f) }
		);
		lr.colorGradient = gradient;

		// Set line width
		lr.startWidth = gridLineWidth;
		lr.endWidth = gridLineWidth;

		// Ensure proper sorting and material
		lr.sortingOrder = -1;
		lr.useWorldSpace = true;
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
