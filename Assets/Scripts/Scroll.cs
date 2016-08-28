using UnityEngine;
using System.Collections.Generic;
using System;

public class Scroll : MonoBehaviour
{

	public static Scroll clickedScroll = null;

	#region Public fields

	public Vector3 endPosition;
	public int scrollIndex;
	public SpriteRenderer sprite;
	public Color activeColor;

	#endregion

	#region Private fields

	private Vector3 startPosition;

	private GoTween moveAnimation = null;

	private OpenedScroll openedScroll;

	#endregion

	void Awake()
	{
		startPosition = transform.position;
		openedScroll = FindObjectOfType<OpenedScroll>();
	}

	void OnMouseEnter()
	{
		if (moveAnimation != null)
			moveAnimation.complete();
		moveAnimation = Go.to(transform, .4f, new GoTweenConfig().position(endPosition).setEaseType(GoEaseType.BackInOut));
	}

	void OnMouseExit()
	{
		if (moveAnimation != null)
			moveAnimation.complete();
		moveAnimation = Go.to(transform, .4f, new GoTweenConfig().position(startPosition).setEaseType(GoEaseType.BackInOut));
	}

	void OnMouseUp()
	{
		if (clickedScroll != this)
		{
			DeactivateClickedScroll();
			// open scroll
			sprite.color = activeColor;
			clickedScroll = this;
			openedScroll.Open();
		}
		else
		{
			// close
			sprite.color = Color.white;
			clickedScroll = null;
			openedScroll.AnimateClose();
		}
	}

	public static void DeactivateClickedScroll()
	{
		if (clickedScroll != null)
		{
			clickedScroll.sprite.color = Color.white;
		}
		clickedScroll = null;
	}
}
