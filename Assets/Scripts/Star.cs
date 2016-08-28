using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour
{

	public ParticleSystem starDefault;
	public ParticleSystem starActive;
	public Renderer littleStar;

	public bool IsSelected { get; private set; }
	public bool IsActive { get; private set; }

	public int ConnectionCount { get; set; }

	public void SelectAndActivate()
	{
		IsSelected = true;
		IsActive = true;
		starDefault.Pause();
		starActive.Play();
		littleStar.enabled = false;
	}

	public void Deselect()
	{
		IsSelected = false;
		if (ConnectionCount == 0)
		{
			Deactivate();
		}
	}

	public void Deactivate()
	{
		if (ConnectionCount > 0)
		{
			ConnectionCount--;
		}
		if (ConnectionCount == 0)
		{
			IsActive = false;
			starActive.Stop();
			starDefault.Play();
			littleStar.enabled = true;
		}
	}
	
}
