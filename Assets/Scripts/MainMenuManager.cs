using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	[Header("UI Elements")]
	public Button playButton;
	public Button quitButton;
	public Text titleText;

	private void Start()
	{
		SetupMainMenu();
	}

	private void SetupMainMenu()
	{
		if (playButton != null)
			playButton.onClick.AddListener(StartGame);

		if (quitButton != null)
			quitButton.onClick.AddListener(QuitGame);

		if (titleText != null)
			titleText.text = "Puzzle Block Game";
	}

	private void StartGame()
	{
		SceneManager.LoadScene("GameScene");
	}

	private void QuitGame()
	{
		Application.Quit();
	}
}