using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
	[Header("Spawn Settings")]
	public GameObject piecePrefab;
	public Transform[] spawnPoints;
	public Color[] pieceColors;
	public AudioClip spawnClip;
	public AudioSource sfxSource;

	[Header("Piece Settings")]
	public PieceType[] availablePieces = { PieceType.LShape , PieceType.Line3 , PieceType.Line4 , PieceType.Square, PieceType.Plane};

	private GamePiece[] currentPieces;

	public void Initialize()
	{
		currentPieces = new GamePiece[spawnPoints.Length];
		GenerateNewPieces();
	}

	public void GenerateNewPieces()
	{
		sfxSource.PlayOneShot(spawnClip);
		// Clear existing pieces
		for (int i = 0; i < currentPieces.Length; i++)
		{
			if (currentPieces[i] != null)
			{
				Destroy(currentPieces[i].gameObject);
			}
		}

		// Generate new pieces
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			currentPieces[i] = CreateRandomPiece(spawnPoints[i]);
		}
	}

	private GamePiece CreateRandomPiece( Transform spawnPoint )
	{
		GameObject pieceObj = Instantiate(piecePrefab , spawnPoint.position , Quaternion.identity);
		GamePiece piece = pieceObj.GetComponent<GamePiece>();

		// Set random type and color
		piece.pieceType = availablePieces[Random.Range(0 , availablePieces.Length)];
		piece.pieceColor = pieceColors[Random.Range(0 , pieceColors.Length)];
		piece.SetupPiece(piece.pieceType , piece.pieceColor);
		return piece;
	}

	public bool CanAnyPieceFit()
	{
		foreach (GamePiece piece in currentPieces)
		{
			if (piece != null && piece.CanFitOnGrid())
			{
				return true;
			}
		}
		return false;
	}
}