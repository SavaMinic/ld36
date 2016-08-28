using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{

	public Button resetButton;
	public Button skipButton;

	private Text skipText;

	public float skipTime;

	void Awake()
	{
		HideButtons();
		skipText = skipButton.GetComponentInChildren<Text>();
	}

	public void OnResetClick()
	{
		ConnectionManager.Instance.DeleteAllConnections();
		StarManager.Instance.ResetSky();
	}

	public void OnSkipClick()
	{
		StarManager.Instance.Skip();
		skipButton.interactable = false;
		skipText.fontSize = 36;
		skipTime = Time.time + 60f;
	}

	void Update()
	{
		if (!skipButton.interactable)
		{
			if (Time.time >= skipTime)
			{
				skipButton.interactable = true;
				skipText.text = "Skip";
				skipText.fontSize = 48;
			}
			else
			{
				skipText.text = "Skip in " + (int)(skipTime - Time.time) + "s";
			}
		}
	}

	public void ShowButtons()
	{
		resetButton.gameObject.SetActive(true);
		skipButton.gameObject.SetActive(true);
	}

	public void HideButtons()
	{
		resetButton.gameObject.SetActive(false);
		skipButton.gameObject.SetActive(false);
	}

}
