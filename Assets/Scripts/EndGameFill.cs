using UnityEngine;
using System.Collections;

public class EndGameFill : MonoBehaviour
{

	private SpriteRenderer sprite;
	private Color transparent = new Color(1f, 1f, 1f, 0f);

	void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		gameObject.SetActive(false);
	}

	void OnMouseUp()
	{
		if (gameObject.activeSelf)
		{
			KingManager.Instance.NextTalk();
		}
	}

	public void Show()
	{
		gameObject.SetActive(true);
		Go.to(sprite, .4f, new GoTweenConfig().colorProp("color", Color.white));
	}

	public void Hide()
	{
		Go.to(sprite, .4f, new GoTweenConfig().colorProp("color", transparent));
		gameObject.SetActive(false);
	}
}
