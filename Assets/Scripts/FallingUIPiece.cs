using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class FallingUIPiece : MonoBehaviour, IPointerClickHandler
{
	private float fallSpeed;
	private float rotationSpeed;
	private float lifetime;
	private float spawnTime;
	private RectTransform rectTransform;
	private TetrisRainUI spawner;
	private CanvasGroup canvasGroup;
	public UnityEvent onClickOnTetrisPiece;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();

		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}

		canvasGroup.blocksRaycasts = true;
	}

	public void Initialize( Vector2 startPos , float startRotation , float speed , float rotSpeed , float life , TetrisRainUI rainSpawner )
	{
		rectTransform.anchoredPosition = startPos;
		rectTransform.rotation = Quaternion.Euler(0 , 0 , startRotation);
		fallSpeed = speed;
		rotationSpeed = rotSpeed;
		lifetime = life;
		spawner = rainSpawner;
		spawnTime = Time.time;
	}

	public bool UpdatePiece( float deltaTime , float destroyBound )
	{
		if (Time.time - spawnTime > lifetime)
			return false;

		Vector2 pos = rectTransform.anchoredPosition;
		pos.y -= fallSpeed * deltaTime;
		rectTransform.anchoredPosition = pos;
		rectTransform.Rotate(0 , 0 , rotationSpeed * deltaTime , Space.Self);

		return pos.y > destroyBound;
	}

	public void OnPointerClick( PointerEventData eventData )
	{
		DisablePiece();

	}

	public void OnMouseDown()
	{
		DisablePiece();
	}

	private void DisablePiece()
	{
		if (spawner != null)
		{
			spawner.OnPieceClicked(this);
		}
		onClickOnTetrisPiece.Invoke();
		gameObject.SetActive(false);
	}
}