﻿using UnityEngine;
using System.Collections;
using System;

public class Connection : MonoBehaviour
{

	public Star from { get; private set; }

	public Star to { get; private set;}

	private float width;

	public void Initialize(Star fromStar, bool isSolution = false)
	{
		from = fromStar;
		this.width = 0.3f;
		if (isSolution)
		{
			GetComponent<MeshRenderer>().material.color = Color.red;
			this.width = 0.2f;
		}
	}

	public void LookAt(Vector3 to)
	{
		Vector3 start = from.transform.position;
		var dir = to - start;
		var position = start + (dir / 2f);

		transform.position = position;
		transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
		transform.localScale = new Vector3(width, dir.magnitude / 2f, width);
	}

	public void Connect(Star toStar)
	{
		to = toStar;
		LookAt(to.transform.position);
	}


}
