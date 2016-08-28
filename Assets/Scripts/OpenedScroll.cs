using UnityEngine;
using System.Collections.Generic;
using System;

public class OpenedScroll : MonoBehaviour
{

	private Renderer myRenderer;

	private Vector3 defaultScale;

	private GoTween toggleAnimation;

	void Awake()
	{
		defaultScale = transform.localScale;
		myRenderer = GetComponent<Renderer>();
		myRenderer.enabled = false;
	}

	public void Open()
	{
		if (toggleAnimation != null)
			toggleAnimation.complete();
		if (myRenderer.enabled)
		{
			AnimateClose(AnimateOpening);
		}
		else
		{
			AnimateOpening();
		}
	}

	public void AnimateClose(Action callbackOnClosed = null)
	{
		var endScale = new Vector3(0f, defaultScale.y, defaultScale.z);
		toggleAnimation = Go.to(transform, 0.5f,new GoTweenConfig()
			.vector3Prop("localScale", endScale).setEaseType(GoEaseType.BackInOut)
			.onComplete(tw =>
			{
				myRenderer.enabled = false;
				if (callbackOnClosed != null)
					callbackOnClosed();
			})
		);
	}

	public void AnimateOpening()
	{
		transform.localScale = new Vector3(0f, defaultScale.y, defaultScale.z);
		toggleAnimation = Go.to(transform, 0.5f, new GoTweenConfig().vector3Prop("localScale", defaultScale).setEaseType(GoEaseType.BackInOut));
		myRenderer.enabled = true;
	}

}
