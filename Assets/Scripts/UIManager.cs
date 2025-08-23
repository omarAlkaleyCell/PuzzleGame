using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[Header("UI Elements")]
	public TextMeshProUGUI scoreText;
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

	public void ShowGameOver( int finalScore )
	{
		gameOverPanel.SetActive(true);
		if (finalScoreText != null)
			finalScoreText.text = "Final Score: " + finalScore.ToString();
	}
}