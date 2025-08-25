using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
	public Image musicIcon;             // Icon inside the button
	public Sprite soundOnIcon;          // Speaker icon
	public Sprite soundOffIcon;         // Muted icon
	public Button musicButton;

	[Header("Audio")]
	public AudioSource musicSource;     // The music AudioSource

	private bool isMuted = false;

	private void Start()
	{
		// Set initial icon
		UpdateIcon();

		// Add listener to button
		musicButton.onClick.AddListener(ToggleMusic);
	}

	private void ToggleMusic()
	{
		isMuted = !isMuted;
		musicSource.mute = isMuted;
		UpdateIcon();
	}

	private void UpdateIcon()
	{
		musicIcon.sprite = isMuted ? soundOffIcon : soundOnIcon;
	}
}
