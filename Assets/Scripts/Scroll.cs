using UnityEngine;
using System.Collections.Generic;
using System;

public class Scroll : MonoBehaviour
{

	public Vector3 endPosition;

	private Vector3 startPosition;

	private GoTween moveAnimation = null;

	void Awake()
	{
		startPosition = transform.position;
	}

	void OnMouseEnter() {
		if (moveAnimation != null)
			moveAnimation.complete();
		moveAnimation = Go.to(transform, .4f, new GoTweenConfig().position(endPosition).setEaseType(GoEaseType.BackInOut));
	}

	void OnMouseExit() {
		if (moveAnimation != null)
			moveAnimation.complete();
		moveAnimation = Go.to(transform, .4f, new GoTweenConfig().position(startPosition).setEaseType(GoEaseType.BackInOut));
	}
}
