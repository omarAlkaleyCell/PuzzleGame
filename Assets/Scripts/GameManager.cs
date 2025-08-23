using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("Game Components")]
	public GridManager gridManager;
	public PieceSpawner pieceSpawner;
	public UIManager uiManager;

	[Header("Game Settings")]
	public int baseScorePerLine = 100;
	public int comboMultiplier = 50;

	private int currentScore = 0;
	private bool isGameOver = false;

	public static GameManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		InitializeGame();
	}

	private void InitializeGame()
	{
		gridManager.Initialize();
		pieceSpawner.Initialize();
		uiManager.UpdateScore(currentScore);
	}

	public void OnPiecePlaced( GamePiece piece )
	{
		int linesCleared = gridManager.CheckAndClearLines();

		if (linesCleared > 0)
		{
			int scoreToAdd = CalculateScore(linesCleared);
			AddScore(scoreToAdd);
		}

		pieceSpawner.GenerateNewPieces();
		CheckGameOver();
	}

	private int CalculateScore( int linesCleared )
	{
		return baseScorePerLine * linesCleared + ( comboMultiplier * ( linesCleared - 1 ) );
	}

	private void AddScore( int points )
	{
		currentScore += points;
		uiManager.UpdateScore(currentScore);
	}

	private void CheckGameOver()
	{
		if (!pieceSpawner.CanAnyPieceFit())
		{
			GameOver();
		}
	}

	private void GameOver()
	{
		isGameOver = true;
		uiManager.ShowGameOver(currentScore);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}