using UnityEngine;
using System.Collections;

public class Connection : MonoBehaviour
{

	public Star from {get; private set; }

	public Star to {get; private set;}

	public void Initialize(Star fromStar, Star toStar, float width = 0.3f)
	{
		from = fromStar;
		to = toStar;

		Vector3 start = fromStar.transform.position;
		var dir = toStar.transform.position - start;
		var position = start + (dir / 2f);

		transform.position = position;
		transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
		transform.localScale = new Vector3(width, dir.magnitude / 2f, width);

		transform.parent = GameObject.Find("Connections").transform;
	}

}
