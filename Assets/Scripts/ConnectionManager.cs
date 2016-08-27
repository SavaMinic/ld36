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
		
		currentConnection.Connect(to);
		connections.Add(currentConnection);
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

	public void DeleteLastConnection()
	{
		if (connections.Count > 0)
		{
			var connection = connections[connections.Count - 1];
			connections.Remove(connection);
			GameObject.Destroy(connection.gameObject);
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
