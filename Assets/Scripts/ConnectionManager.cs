using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
	#region Properties

	public static ConnectionManager Instance { get; private set; }

	#endregion

	#region Private fields

	private List<Connection> connections = new List<Connection>();
	private List<Connection> finalConnections = new List<Connection>();

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
			finalConnections.AddRange(connections);
			connections.Clear();
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
			// delete last added connection
			var lastConnection = connections[connections.Count - 1];
			connections.Remove(lastConnection);
			GameObject.Destroy(lastConnection.gameObject);

			// delete current connection
			DeleteCurrentConnection();
			// and make a new one
			CreateCurrentConnection(lastConnection.from);
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
		for (int i = 0; i < finalConnections.Count; i++)
		{
			GameObject.Destroy(finalConnections[i].gameObject);
		}
		finalConnections.Clear();
	}

}
