using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{

	public void OnResetClick()
	{
		ConnectionManager.Instance.DeleteAllConnections();
		StarManager.Instance.ResetSky();
	}

}
