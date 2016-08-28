using UnityEngine;
using System.Collections;

public class LittleConnection : MonoBehaviour
{

	void Start ()
	{
		// works only with Sprite shaders (which doesn't use z-order)
		var myRenderer = GetComponent<MeshRenderer>();
		myRenderer.sortingLayerID = SortingLayer.NameToID("LittleConnection");
		myRenderer.sortingLayerName = "LittleConnection";
		myRenderer.sortingOrder = 0;
	}
}
