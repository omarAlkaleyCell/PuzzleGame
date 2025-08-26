using System;
using UnityEngine;
using UnityEngine.Events;
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
	public int scorePerLevel = 1000;

	private int currentScore = 0;
	private int currentLevel = 1;
	private bool isGameOver = false;

	[Header("SFX")]
	public AudioClip[] addScoreClips;
	public AudioClip placingClip;
	public AudioClip[] levelUpClips;
	public AudioClip GameOverClip;
	public AudioSource sfxSource;
	public AudioSource sfx2Source;
	
	[Header("Music")]
	public AudioClip[] musicClips;
	public AudioSource musicSource;
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

	public UnityEvent onGainScore;
	private void Start()
	{
		InitializeGame();
	}

	private void InitializeGame()
	{
		gridManager.Initialize();
		pieceSpawner.Initialize();
		uiManager.UpdateScore(currentScore);
		uiManager.UpdateLevel(currentLevel);
		uiManager.UpdateProgressBar(0f);
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
		
		sfxSource.PlayOneShot(placingClip);
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
		CheckLevelProgression();
		sfxSource.PlayOneShot(addScoreClips[UnityEngine.Random.Range(0 , addScoreClips.Length)]);
		onGainScore.Invoke();
	}

	private void CheckLevelProgression()
	{
		int targetScore = currentLevel * scorePerLevel;
		float progress = (float) ( currentScore % scorePerLevel ) / scorePerLevel;

		uiManager.UpdateProgressBar(progress);

		if (currentScore >= targetScore)
		{
			currentLevel++;
			sfx2Source.PlayOneShot(levelUpClips[UnityEngine.Random.Range(0 , maxExclusive: levelUpClips.Length)]);
			musicSource.clip = musicClips[UnityEngine.Random.Range(0 , maxExclusive: musicClips.Length)];
			musicSource.Play();
			uiManager.UpdateLevel(currentLevel);
			// TODO: Add level up effects
		}
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
		sfx2Source.PlayOneShot(GameOverClip);
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