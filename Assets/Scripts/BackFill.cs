using UnityEngine;
using System.Collections;

public class BackFill : MonoBehaviour
{

	private OpenedScroll scroll;
	private SpriteRenderer sprite;
	private Color transparent = new Color(1f, 1f, 1f, 0f);

	void Start()
	{
		scroll = FindObjectOfType<OpenedScroll>();
		sprite = GetComponent<SpriteRenderer>();
	}

	void OnMouseUp()
	{
		if (gameObject.activeSelf)
		{
			scroll.AnimateClose();
			Scroll.DeactivateClickedScroll();
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
