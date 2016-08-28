using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
	#region Properties

	public static ConnectionManager Instance { get; private set; }

	public List<Connection> Connections { get; private set; }

	#endregion

	#region Private fields


	private GameObject connectionPrefab;

	private Connection currentConnection;

	#endregion

	void Awake()
	{
		Instance = this;
		connectionPrefab = Resources.Load("Prefabs/Connection") as GameObject;
		Connections = new List<Connection>();
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

	public void ConnectCurrentConnection(Star to, bool createNextConnection = true)
	{
		if (currentConnection == null)
			return;

		// check if this connection already exists
		if (!Connections.Exists(c => 
			(c.from == currentConnection.from && c.to == to)
		    || (c.from == to && c.to == currentConnection.from)
		    ))
		{
			currentConnection.Connect(to);
			Connections.Add(currentConnection);
		} else
			DeleteCurrentConnection();

		if (createNextConnection)
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

	public void CreateConnection(Star from, Star to)
	{
		var connection = Instantiate<GameObject>(connectionPrefab).GetComponent<Connection>();
		connection.Initialize(from);
		connection.Connect(to);
		Debug.Log("CONNECTION " + from.ID + " " + to.ID);
		Connections.Add(connection);
	}

	#endregion

	public void DeleteAllConnections()
	{
		for (int i = 0; i < Connections.Count; i++)
		{
			GameObject.Destroy(Connections[i].gameObject);
		}
		Connections.Clear();
	}

}
