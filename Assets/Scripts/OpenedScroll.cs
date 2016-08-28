using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class OpenedScroll : MonoBehaviour
{
	
	#region Public fields

	public float minX, maxX;
	public float minY, maxY;

	#endregion

	#region Private fields

	private Renderer myRenderer;

	private Vector3 defaultScale;

	private GoTween toggleAnimation;
	private bool showedSolution;

	private BackFill backFill;

	private GameObject littleStarPrefab;
	private List<GameObject> littleStars = new List<GameObject>();

	#endregion

	void Awake()
	{
		defaultScale = transform.localScale;
		myRenderer = GetComponent<Renderer>();
		myRenderer.enabled = false;
		backFill = FindObjectOfType<BackFill>();
		littleStarPrefab = Resources.Load<GameObject>("Prefabs/LittleStar");
	}

	public void Open()
	{
		if (toggleAnimation != null)
		{
			toggleAnimation.complete();
			toggleAnimation.destroy();
		}
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
		backFill.Hide();
		// delete all stars, if any
		littleStars.ForEach(obj => GameObject.Destroy(obj) );
	}

	public void AnimateOpening()
	{
		transform.localScale = new Vector3(0f, defaultScale.y, defaultScale.z);

		// start opening up animation
		showedSolution = false;
		toggleAnimation = Go.to(transform, 0.5f, 
			new GoTweenConfig().vector3Prop("localScale", defaultScale).setEaseType(GoEaseType.BackInOut)
			.onUpdate(tw => {
				// after some time, show solution
				if (!showedSolution && tw.totalElapsedTime > tw.totalDuration * 0.6f)
				{
					showedSolution = true;
					DisplaySolution();
				}
			})
		);
		myRenderer.enabled = true;
		backFill.Show();
	}

	private void DisplaySolution()
	{
		// draw all stars in solution
		var stars = new HashSet<Star>();
		var solution = SolutionManager.Instance.Solutions[Scroll.clickedScroll.scrollIndex];
		for (int i = 0; i < solution.Count; i++)
		{
			stars.Add(solution[i].From);
			stars.Add(solution[i].To);
		}
		// generate stars
		foreach (var star in stars)
		{
			var littleStar = Instantiate<GameObject>(littleStarPrefab);
			littleStar.transform.SetParent(transform);

			var bigPosition = star.gameObject.transform.position;
			var ratioX = (bigPosition.x - StarManager.Instance.minX) /
				(StarManager.Instance.maxX - StarManager.Instance.minX);
			var ratioY = (bigPosition.y - StarManager.Instance.minY) /
				(StarManager.Instance.maxY - StarManager.Instance.minY);

			var newX = minX + (maxX - minX) * ratioX;
			var newy = minY + (maxY - minY) * ratioY;
			littleStar.transform.position = new Vector3(newX, newy, -9f);
			littleStars.Add(littleStar);
		}
	}

}
