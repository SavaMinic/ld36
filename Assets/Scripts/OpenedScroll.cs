using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class OpenedScroll : MonoBehaviour
{
	
	#region Public fields

	public float minX, maxX;
	public float minY, maxY;

	public float littleConnectionWidth;

	public Text answerText;
	public Color answerColor;
	public Text questionText;

	#endregion

	#region Private fields

	private Renderer myRenderer;

	private Vector3 defaultScale;

	private GoTween toggleAnimation;
	private bool showedSolution;

	private BackFill backFill;

	private GameObject littleStarPrefab;
	private List<GameObject> littleStars = new List<GameObject>();

	private GameObject littleConnectionPrefab;
	private List<GameObject> littleConnections = new List<GameObject>();

	private Color noTextColor =new Color(1f,1f,1f,0f);

	#endregion

	void Awake()
	{
		defaultScale = transform.localScale;
		myRenderer = GetComponent<Renderer>();
		myRenderer.enabled = false;
		backFill = FindObjectOfType<BackFill>();
		littleStarPrefab = Resources.Load<GameObject>("Prefabs/LittleStar");
		littleConnectionPrefab = Resources.Load<GameObject>("Prefabs/LittleConnection");
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
		Go.to(answerText, 0.3f, new GoTweenConfig().colorProp("color", noTextColor));
		Go.to(questionText, 0.3f, new GoTweenConfig().colorProp("color", noTextColor));

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
		// delete all connections, if any
		littleConnections.ForEach(obj => GameObject.Destroy(obj) );
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
		if (Scroll.clickedScroll == null)
			return;
		var index = Scroll.clickedScroll.scrollIndex;

		// show answer
		answerText.text = StarManager.Instance.CurrentAnswers[index];
		Go.to(answerText, 0.3f, new GoTweenConfig().colorProp("color", answerColor));
		questionText.text = StarManager.Instance.CurrentProblem;
		Go.to(questionText, 0.3f, new GoTweenConfig().colorProp("color", Color.white));
		
		// draw all stars in solution
		var stars = new HashSet<Star>();
		var solution = SolutionManager.Instance.Solutions[index];
		for (int i = 0; i < solution.Count; i++)
		{
			stars.Add(solution[i].From);
			stars.Add(solution[i].To);
		}

		// generate stars
		var positions = new Dictionary<Star, Vector3>();
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

			// save the position later for possible connections
			positions.Add(star, littleStar.transform.position);
		}


		// generate little connections
		for (int i = 0; i < solution.Count; i++)
		{
			Vector3 start = positions[solution[i].From];
			Vector3 end = positions[solution[i].To];

			var dir = end - start;
			var position = start + (dir / 2f);

			var littleConnection = Instantiate<GameObject>(littleConnectionPrefab);
			littleConnection.transform.position = position;
			littleConnection.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
			littleConnection.transform.localScale = new Vector3(littleConnectionWidth, dir.magnitude / 2f, littleConnectionWidth);
			littleConnection.transform.SetParent(transform);
			littleConnections.Add(littleConnection);
		}
	}

}
