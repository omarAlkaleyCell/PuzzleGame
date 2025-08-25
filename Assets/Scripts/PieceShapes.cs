using UnityEngine;

public enum PieceType
{
	LShape,
	Line3,
	Line4,
	Square,
	Plane
}

public static class PieceShapes
{
	public static Vector2Int[] GetShape( PieceType type )
	{
		switch (type)
		{
			case PieceType.LShape:
				return new Vector2Int[]
				{
					new Vector2Int(0, 0),
					new Vector2Int(0, 1),
					new Vector2Int(0, 2),
					new Vector2Int(1, 0)
				};

			case PieceType.Line3:
				return new Vector2Int[]
				{
					new Vector2Int(0, 0),
					new Vector2Int(1, 0),
					new Vector2Int(2, 0)
				};

			case PieceType.Line4:
				return new Vector2Int[]
				{
					new Vector2Int(0, 0),
					new Vector2Int(1, 0),
					new Vector2Int(2, 0),
					new Vector2Int(3, 0)
				};

			case PieceType.Square:
				return new Vector2Int[]
				{
					new Vector2Int(0, 0),
					new Vector2Int(1, 0),
					new Vector2Int(0, 1),
					new Vector2Int(1, 1)
				};

			case PieceType.Plane:
				return new Vector2Int[]
				{
					new Vector2Int(0, 0),
					new Vector2Int(1, 0),
					new Vector2Int(2, 0),
					new Vector2Int(1, 1)
				};
			default:
				return new Vector2Int[] { Vector2Int.zero };
		}
	}
}