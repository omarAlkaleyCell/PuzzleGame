using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisRainUI : MonoBehaviour
{
	[Header("Tetris Pieces")]
	[SerializeField] private GameObject[] tetrisPiecePrefabs;

	[Header("Spawn Settings")]
	[SerializeField] private float spawnInterval = 0.8f;
	[SerializeField] private float spawnIntervalVariation = 0.3f;
	[SerializeField] private float spawnHeight = 50f;
	[SerializeField] private int maxActivePieces = 15;
	[SerializeField] private RectTransform spawnArea;

	[Header("Piece Physics")]
	[SerializeField] private Vector2 fallSpeedRange = new Vector2(80f , 150f);
	[SerializeField] private Vector2 rotationSpeedRange = new Vector2(20f , 60f);
	[SerializeField] private float pieceLifetime = 8f;

	[Header("Performance")]
	[SerializeField] private int poolSize = 20;
	[SerializeField] private float updateRate = 0.016f;

	[Header("Controls")]
	[SerializeField] private bool autoStart = true;

	private bool isSpawning = false;
	private Queue<GameObject>[] piecePools;
	private List<FallingUIPiece> activePieces = new List<FallingUIPiece>();
	private Coroutine spawnCoroutine;
	private RectTransform canvasRect;
	private float lastUpdateTime;
	private float canvasHalfHeight;
	private float canvasHalfWidth;

	void Start()
	{
		Application.targetFrameRate = 60;

		if (tetrisPiecePrefabs == null || tetrisPiecePrefabs.Length == 0)
			return;

		Canvas canvas = GetComponentInParent<Canvas>();
		if (canvas == null)
			return;

		canvasRect = canvas.GetComponent<RectTransform>();
		if (spawnArea == null)
			spawnArea = canvasRect;

		canvasHalfHeight = canvasRect.rect.height * 0.5f;
		canvasHalfWidth = spawnArea.rect.width * 0.5f;

		InitializePools();

		if (autoStart)
			StartRain();
	}

	void Update()
	{
		if (Time.time - lastUpdateTime >= updateRate)
		{
			UpdatePieces();
			lastUpdateTime = Time.time;
		}
	}

	void InitializePools()
	{
		piecePools = new Queue<GameObject>[tetrisPiecePrefabs.Length];

		for (int i = 0; i < tetrisPiecePrefabs.Length; i++)
		{
			if (tetrisPiecePrefabs[i] == null)
				continue;

			piecePools[i] = new Queue<GameObject>();

			for (int j = 0; j < poolSize / tetrisPiecePrefabs.Length; j++)
			{
				GameObject piece = Instantiate(tetrisPiecePrefabs[i] , spawnArea);
				piece.SetActive(false);

				RectTransform rectTransform = piece.GetComponent<RectTransform>();
				if (rectTransform == null)
					rectTransform = piece.AddComponent<RectTransform>();

				FallingUIPiece fallingComponent = piece.GetComponent<FallingUIPiece>();
				if (fallingComponent == null)
					fallingComponent = piece.AddComponent<FallingUIPiece>();

				piecePools[i].Enqueue(piece);
			}
		}
	}

	public void StartRain()
	{
		if (isSpawning) return;

		isSpawning = true;
		spawnCoroutine = StartCoroutine(SpawnLoop());
	}

	public void StopRain()
	{
		if (!isSpawning) return;

		isSpawning = false;
		if (spawnCoroutine != null)
		{
			StopCoroutine(spawnCoroutine);
			spawnCoroutine = null;
		}
	}

	public void ClearAllPieces()
	{
		for (int i = activePieces.Count - 1; i >= 0; i--)
		{
			if (activePieces[i] != null)
				ReturnToPool(activePieces[i]);
		}
		activePieces.Clear();
	}

	public void OnPieceClicked( FallingUIPiece piece )
	{
		if (piece != null && activePieces.Contains(piece))
		{
			activePieces.Remove(piece);
			ReturnToPool(piece);
		}
	}

	private IEnumerator SpawnLoop()
	{
		while (isSpawning)
		{
			if (activePieces.Count < maxActivePieces)
				SpawnPiece();

			float waitTime = spawnInterval + Random.Range(-spawnIntervalVariation , spawnIntervalVariation);
			yield return new WaitForSeconds(Mathf.Max(0.1f , waitTime));
		}
	}

	private void SpawnPiece()
	{
		int pieceIndex = Random.Range(0 , tetrisPiecePrefabs.Length);

		if (piecePools[pieceIndex] == null || piecePools[pieceIndex].Count == 0)
			return;

		GameObject piece = piecePools[pieceIndex].Dequeue();
		if (piece == null) return;

		FallingUIPiece fallingComponent = piece.GetComponent<FallingUIPiece>();
		if (fallingComponent == null)
			return;

		Vector2 spawnPosition = new Vector2(
			Random.Range(-canvasHalfWidth , canvasHalfWidth) ,
			canvasHalfHeight + spawnHeight
		);

		fallingComponent.Initialize(
			spawnPosition ,
			Random.Range(0f , 360f) ,
			Random.Range(fallSpeedRange.x , fallSpeedRange.y) ,
			Random.Range(rotationSpeedRange.x , rotationSpeedRange.y) * ( Random.value > 0.5f ? 1 : -1 ) ,
			pieceLifetime ,
			this
		);

		piece.SetActive(true);
		activePieces.Add(fallingComponent);
	}

	private void UpdatePieces()
	{
		float deltaTime = Time.deltaTime;
		float destroyBound = -canvasHalfHeight - 100f;

		for (int i = activePieces.Count - 1; i >= 0; i--)
		{
			if (activePieces[i] == null || !activePieces[i].gameObject.activeInHierarchy)
			{
				activePieces.RemoveAt(i);
				continue;
			}

			if (!activePieces[i].UpdatePiece(deltaTime , destroyBound))
			{
				ReturnToPool(activePieces[i]);
				activePieces.RemoveAt(i);
			}
		}
	}

	public void ReturnToPool( FallingUIPiece piece )
	{
		if (piece == null || !piece.gameObject.activeInHierarchy) return;

		piece.gameObject.SetActive(false);

		for (int i = 0; i < tetrisPiecePrefabs.Length; i++)
		{
			if (piece.gameObject.name.Contains(tetrisPiecePrefabs[i].name))
			{
				piecePools[i].Enqueue(piece.gameObject);
				break;
			}
		}
	}
}