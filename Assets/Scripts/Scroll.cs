using UnityEngine;
using System.Collections.Generic;
using System;

public class Scroll : MonoBehaviour
{

	private static int openedScrollIndex = -1;

	public Vector3 endPosition;
	public int scrollIndex;

	private Vector3 startPosition;

	private GoTween moveAnimation = null;

	private OpenedScroll openedScroll;

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
		if (openedScrollIndex != scrollIndex)
		{
			// open scroll
			openedScrollIndex = scrollIndex;
			openedScroll.Open();
		}
		else
		{
			// close
			openedScrollIndex = -1;
			openedScroll.AnimateClose();
		}
	}
}
