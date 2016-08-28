using UnityEngine;
using System.Collections;
using System;

public class Star : MonoBehaviour
{

	private static int sID = 0;


	public ParticleSystem starDefault;
	public ParticleSystem starActive;
	public Renderer littleStar;

	public int ID { get; private set; }
	public bool IsActive { get; private set; }

	void Awake()
	{
		ID = ++sID;
	}

	public void Activate()
	{
		IsActive = true;
		starDefault.Pause();
		starActive.Play();
		littleStar.enabled = false;
	}

	public void Deactivate()
	{
		IsActive = false;
		starActive.Stop();
		starDefault.Play();
		littleStar.enabled = true;
	}

}
