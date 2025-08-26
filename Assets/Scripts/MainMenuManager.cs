using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private Button playButton;
	[SerializeField] private Button quitButton;
	[SerializeField] private Button settingsButton;
	[SerializeField] private GameObject settingsPanel;
	[SerializeField] private Text titleText;

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
		
		if (settingsButton != null)
			settingsButton.onClick.AddListener(OpenSettings);
	}

	private void StartGame()
	{
		SceneManager.LoadScene("GameScene");
	}

	private void QuitGame()
	{
		Application.Quit();
	}

	private void OpenSettings()
	{
		SetButtonsActivation(false);
		settingsPanel.SetActive(true);
	}

	private void SetButtonsActivation(bool value)
	{
		playButton.gameObject.SetActive(value);
		quitButton.gameObject.SetActive(value);
		settingsButton.gameObject.SetActive(value);
	}

	public void CloseSettings()
	{
		SetButtonsActivation(true);
		settingsPanel.SetActive(false);
	}
}