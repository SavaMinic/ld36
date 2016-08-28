using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
	#region Properties

	public static ConnectionManager Instance { get; private set; }

	#endregion

	#region Private fields

	private List<Connection> connections = new List<Connection>();

	private GameObject connectionPrefab;

	private Connection currentConnection;

	#endregion

	void Awake()
	{
		Instance = this;
		connectionPrefab = Resources.Load("Prefabs/Connection") as GameObject;
	}

	void Update()
	{
		// Released the button while dragging
		if (Input.GetMouseButtonUp(0) && StarManager.Instance.State == StarManager.GameState.Dragging)
		{
			if (currentConnection != null)
			{
				GameObject.Destroy(currentConnection.gameObject);
				currentConnection = null;
			}
		}
	}

	#region Public methods

	public void CreateCurrentConnection(Star from)
	{
		currentConnection = Instantiate<GameObject>(connectionPrefab).GetComponent<Connection>();
		currentConnection.Initialize(from);
		currentConnection.transform.SetParent(transform);
		UpdateCurrentConnectionPosition();
	}

	public void ConnectCurrentConnection(Star to)
	{
		if (currentConnection == null)
			return;

		// check if this connection already exists
		if (!connections.Exists(c => 
			(c.from == currentConnection.from && c.to == to)
		    || (c.from == to && c.to == currentConnection.from)
		    ))
		{
			currentConnection.Connect(to);
			connections.Add(currentConnection);
		} else
			DeleteCurrentConnection();

		CreateCurrentConnection(to);
	}

	public void UpdateCurrentConnectionPosition()
	{
		if (currentConnection)
		{
			var mousePosition = Input.mousePosition;
			mousePosition.z = 10f;
			currentConnection.LookAt(Camera.main.ScreenToWorldPoint(mousePosition));
		}
	}

	public void DeleteCurrentConnection()
	{
		if (currentConnection != null)
		{
			GameObject.Destroy(currentConnection.gameObject);
			currentConnection = null;
		}
	}

	#endregion

	private void DeleteAllConnections()
	{
		for (int i = 0; i < connections.Count; i++)
		{
			GameObject.Destroy(connections[i].gameObject);
		}
		connections.Clear();
	}

}
