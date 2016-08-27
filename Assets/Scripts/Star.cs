using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour
{

	public bool IsSelected { get; private set; }

	private Renderer myRenderer;

	void Awake()
	{
		myRenderer = GetComponent<Renderer>();
	}

	public void Select()
	{
		IsSelected = true;
		myRenderer.material.color = Color.red;
	}

	public void Deselect()
	{
		IsSelected = false;
		myRenderer.material.color = Color.white;
	}
	
}
