using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[Header("UI Elements")]
	public TextMeshProUGUI scoreText;
	public Slider progressBar;
	public TextMeshProUGUI levelText;
	public GameObject gameOverPanel;
	public TextMeshProUGUI finalScoreText;
	public Button restartButton;
	public Button mainMenuButton;

	private void Start()
	{
		SetupUI();
	}

	private void SetupUI()
	{
		gameOverPanel.SetActive(false);

		if (restartButton != null)
			restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());

		if (mainMenuButton != null)
			mainMenuButton.onClick.AddListener(() => GameManager.Instance.GoToMainMenu());
	}

	public void UpdateScore( int score )
	{
		if (scoreText != null)
			scoreText.text = "Score: " + score.ToString();
	}

	public void UpdateProgressBar( float progress )
	{
		if (progressBar != null)
			progressBar.value = progress;
	}

	public void UpdateLevel( int level )
	{
		if (levelText != null)
			levelText.text = "Level: " + level.ToString();
	}

	public void ShowGameOver( int finalScore )
	{
		gameOverPanel.SetActive(true);
		if (finalScoreText != null)
			finalScoreText.text = "Final Score: " + finalScore.ToString();
	}
}