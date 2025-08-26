using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class MusicManager : MonoBehaviour
{
	public static MusicManager Instance { get; private set; }

	[Header("Audio Sources")]
	private Dictionary<AudioSource , float> musicSources = new Dictionary<AudioSource , float>();
	private Dictionary<AudioSource , float> sfxSources = new Dictionary<AudioSource , float>();

	[Header("Volume Settings")]
	[SerializeField] private float defaultMusicVolume = 0.7f;
	[SerializeField] private float defaultSFXVolume = 0.8f;

	private float currentMusicVolume;
	private float currentSFXVolume;

	private Slider musicSlider;
	private Slider sfxSlider;

	private const float AUDIO_SOURCE_SCAN_DELAY = 0.1f;
	private WaitForSeconds scanDelay;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			scanDelay = new WaitForSeconds(AUDIO_SOURCE_SCAN_DELAY);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	private void Start()
	{
		// Check if PlayerPrefs exist, if not use defaults
		if (!PlayerPrefs.HasKey("MusicVolume"))
		{
			currentMusicVolume = defaultMusicVolume;
			PlayerPrefs.SetFloat("MusicVolume" , defaultMusicVolume);
		}
		else
		{
			currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume" , defaultMusicVolume);
		}

		if (!PlayerPrefs.HasKey("SFXVolume"))
		{
			currentSFXVolume = defaultSFXVolume;
			PlayerPrefs.SetFloat("SFXVolume" , defaultSFXVolume);
		}
		else
		{
			currentSFXVolume = PlayerPrefs.GetFloat("SFXVolume" , defaultSFXVolume);
		}

		// Ensure volumes are never 0 unless intentionally set
		if (currentMusicVolume <= 0f && defaultMusicVolume > 0f)
		{
			currentMusicVolume = defaultMusicVolume;
		}

		if (currentSFXVolume <= 0f && defaultSFXVolume > 0f)
		{
			currentSFXVolume = defaultSFXVolume;
		}

		PlayerPrefs.Save();

		SceneManager.sceneLoaded += OnSceneLoaded;

		StartCoroutine(InitializeAudioSystem());
	}

	private void OnSceneLoaded( Scene scene , LoadSceneMode loadSceneMode )
	{
		StartCoroutine(RefreshAudioSources());
	}

	private IEnumerator InitializeAudioSystem()
	{
		yield return scanDelay;
		RefreshAudioSourcesImmediate();
		SetupSliders();
		ApplyVolumeSettings();
	}

	private IEnumerator RefreshAudioSources()
	{
		yield return scanDelay;
		RefreshAudioSourcesImmediate();
		SetupSliders();
		ApplyVolumeSettings();
	}

	private void RefreshAudioSourcesImmediate()
	{
		CleanupDestroyedSources();

		AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

		int newMusicSources = 0;
		int newSfxSources = 0;

		foreach (AudioSource source in allAudioSources)
		{
			if (source == null) continue;

			if (musicSources.ContainsKey(source) || sfxSources.ContainsKey(source))
				continue;

			string objectName = source.gameObject.name.ToLower();
			string tagName = source.gameObject.tag.ToLower();

			if (IsAudioSourceMusic(objectName , tagName))
			{
				float originalVolume = source.volume;
				musicSources[source] = originalVolume;
				source.volume = originalVolume * currentMusicVolume;
				newMusicSources++;
			}
			else if (IsAudioSourceSFX(objectName , tagName))
			{
				float originalVolume = source.volume;
				sfxSources[source] = originalVolume;
				source.volume = originalVolume * currentSFXVolume;
				newSfxSources++;
			}
		}
	}

	private bool IsAudioSourceMusic( string objectName , string tagName )
	{
		return objectName.Contains("music") || objectName.Contains("bgm") ||
			   objectName.Contains("background") || tagName.Contains("music");
	}

	private bool IsAudioSourceSFX( string objectName , string tagName )
	{
		return objectName.Contains("sfx") || objectName.Contains("sound") ||
			   objectName.Contains("effect") || tagName.Contains("sfx") ||
			   ( !IsAudioSourceMusic(objectName , tagName) );
	}

	private void CleanupDestroyedSources()
	{
		var musicToRemove = new List<AudioSource>();
		foreach (var kvp in musicSources)
		{
			if (kvp.Key == null)
				musicToRemove.Add(kvp.Key);
		}
		foreach (var source in musicToRemove)
			musicSources.Remove(source);

		var sfxToRemove = new List<AudioSource>();
		foreach (var kvp in sfxSources)
		{
			if (kvp.Key == null)
				sfxToRemove.Add(kvp.Key);
		}
		foreach (var source in sfxToRemove)
			sfxSources.Remove(source);
	}

	private void SetupSliders()
	{
		if (musicSlider == null)
		{
			Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsInactive.Include , FindObjectsSortMode.None);
			foreach (Slider slider in allSliders)
			{
				if (slider.gameObject.CompareTag("MusicSlider"))
				{
					musicSlider = slider;
					break;
				}
			}
		}

		if (musicSlider != null)
		{
			musicSlider.minValue = 0f;
			musicSlider.maxValue = 1f;

			musicSlider.onValueChanged.RemoveAllListeners();
			musicSlider.value = currentMusicVolume;
			musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		}

		if (sfxSlider == null)
		{
			Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsInactive.Include , FindObjectsSortMode.None);
			foreach (Slider slider in allSliders)
			{
				if (slider.gameObject.CompareTag("SFXSlider"))
				{
					sfxSlider = slider;
					break;
				}
			}
		}

		if (sfxSlider != null)
		{
			sfxSlider.minValue = 0f;
			sfxSlider.maxValue = 1f;

			sfxSlider.onValueChanged.RemoveAllListeners();
			sfxSlider.value = currentSFXVolume;
			sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
		}
	}

	public void RefreshSliderConnections()
	{
		musicSlider = null;
		sfxSlider = null;
		SetupSliders();
	}

	public void OnMusicVolumeChanged( float volume )
	{
		currentMusicVolume = Mathf.Clamp01(volume);
		ApplyMusicVolume();
		SaveVolumePreferences();
	}

	public void OnSFXVolumeChanged( float volume )
	{
		currentSFXVolume = Mathf.Clamp01(volume);
		ApplySFXVolume();
		SaveVolumePreferences();
	}

	private void ApplyVolumeSettings()
	{
		ApplyMusicVolume();
		ApplySFXVolume();
	}

	private void ApplyMusicVolume()
	{
		CleanupDestroyedSources();

		foreach (var kvp in musicSources)
		{
			if (kvp.Key != null)
			{
				kvp.Key.volume = kvp.Value * currentMusicVolume;
			}
		}
	}

	private void ApplySFXVolume()
	{
		CleanupDestroyedSources();

		foreach (var kvp in sfxSources)
		{
			if (kvp.Key != null)
			{
				kvp.Key.volume = kvp.Value * currentSFXVolume;
			}
		}
	}

	private void SaveVolumePreferences()
	{
		PlayerPrefs.SetFloat("MusicVolume" , currentMusicVolume);
		PlayerPrefs.SetFloat("SFXVolume" , currentSFXVolume);
		PlayerPrefs.Save();
	}

	public void SetMasterMusicVolume( float volume )
	{
		volume = Mathf.Clamp01(volume);
		currentMusicVolume = volume;

		if (musicSlider != null)
			musicSlider.value = volume;

		ApplyMusicVolume();
		SaveVolumePreferences();
	}

	public void SetMasterSFXVolume( float volume )
	{
		volume = Mathf.Clamp01(volume);
		currentSFXVolume = volume;

		if (sfxSlider != null)
			sfxSlider.value = volume;

		ApplySFXVolume();
		SaveVolumePreferences();
	}

	public void MuteMusic() => SetMasterMusicVolume(0f);
	public void UnmuteMusic() => SetMasterMusicVolume(defaultMusicVolume);
	public void MuteSFX() => SetMasterSFXVolume(0f);
	public void UnmuteSFX() => SetMasterSFXVolume(defaultSFXVolume);

	public float GetMusicVolume() => currentMusicVolume;
	public float GetSFXVolume() => currentSFXVolume;

	public void RegisterMusicSource( AudioSource source )
	{
		if (source != null && !musicSources.ContainsKey(source))
		{
			float originalVolume = source.volume;
			musicSources[source] = originalVolume;
			source.volume = originalVolume * currentMusicVolume;
		}
	}

	public void RegisterSFXSource( AudioSource source )
	{
		if (source != null && !sfxSources.ContainsKey(source))
		{
			float originalVolume = source.volume;
			sfxSources[source] = originalVolume;
			source.volume = originalVolume * currentSFXVolume;
		}
	}

	public void UnregisterAudioSource( AudioSource source )
	{
		musicSources.Remove(source);
		sfxSources.Remove(source);
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			Instance = null;
		}
	}

	private void OnApplicationPause( bool pauseStatus )
	{
		if (pauseStatus)
			SaveVolumePreferences();
	}

	private void OnApplicationFocus( bool hasFocus )
	{
		if (!hasFocus)
			SaveVolumePreferences();
	}
}