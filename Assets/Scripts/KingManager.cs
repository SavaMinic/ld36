using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Collections;

public class KingManager : MonoBehaviour
{

	#region Properties

	public static KingManager Instance { get; private set; }

	#endregion

	#region Public fields

	public EndGameFill background;

	public Vector3 talkPosition;

	public SpriteRenderer king;
	public SpriteRenderer bubble;
	public Text bubbleText;

	public float talkDelay;

	#endregion

	#region Private fields

	private Vector3 startPosition;
	private List<string> talks = new List<string>();

	private Color textColor = new Color(0.2f, 0.2f, 0.2f, 1f);
	private Color noTextColor = new Color(0.2f, 0.2f, 0.2f, 0f);
	private Color invisibleColor = new Color(1f, 1f, 1f, 0f);

	#endregion

	void Awake()
	{
		Instance = this;
		startPosition = king.transform.position;
	}

	public void StartTalk(List<string> talks)
	{
		this.talks = talks;
		background.Show();
		Go.to(king.transform, 1.2f, new GoTweenConfig().position(talkPosition).setEaseType(GoEaseType.BackInOut));
		NextTalk();
	}

	public void EndTalk()
	{
		background.Hide();
		Go.to(king.transform, 0.6f, new GoTweenConfig().position(startPosition).setEaseType(GoEaseType.BackInOut));
		Go.to(bubble, 0.3f, new GoTweenConfig().colorProp("color", invisibleColor));
		Go.to(bubbleText, 0.3f, new GoTweenConfig().colorProp("color", noTextColor));
	}

	public void NextTalk()
	{
		if (talks.Count == 0)
		{
			EndTalk();
			StarManager.Instance.StartGame();
		}
		else
		{
			var talk = talks[0];
			talks.RemoveAt(0);
			StartCoroutine(Talk(talk, true));
		}
	}

	IEnumerator Talk(String text, bool closeBubble = false)
	{
		if (closeBubble)
		{
			Go.to(bubble, 0.3f, new GoTweenConfig().colorProp("color", invisibleColor));
			Go.to(bubbleText, 0.3f, new GoTweenConfig().colorProp("color", noTextColor));
		}
		yield return new WaitForSeconds(talkDelay);
		bubbleText.text = text;
		Go.to(bubble, 0.3f, new GoTweenConfig().colorProp("color", Color.white));
		Go.to(bubbleText, 0.3f, new GoTweenConfig().colorProp("color", textColor));
	}

}
