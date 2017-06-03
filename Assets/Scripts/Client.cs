using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour
{
	public GameObject chatContainer;
	public GameObject messagePrefab;
	public GameObject messageInput;
	public string clientName = "caleb";

	private bool socketReady;
	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;

	public void Start()
	{
		ConnectToServer ();
	}

	public void ConnectToServer()
	{
		if (socketReady)
			return;

		string host = "127.0.0.1";
		int port = 6321;

		try
		{
			socket = new TcpClient(host, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;

			if (socketReady)
			{
				writer.WriteLine (clientName);
				writer.Flush ();
			}
		}
		catch(Exception e)
		{
			Debug.Log ("Socket error: " + e.Message);
		}
	}

	private void Update()
	{
		if (socketReady)
		{
			if(stream.DataAvailable)
			{
				string data = reader.ReadLine();
				if (data != null)
					OnIncomingData (data);
			}
		}
	}

	private void OnIncomingData(string data)
	{
		GameObject go = Instantiate (messagePrefab, chatContainer.transform) as GameObject;
		go.GetComponentInChildren<Text> ().text = data;
	}

	public void Send(InputField input)
	{
		if (!socketReady)
			return;

		writer.WriteLine (input.text);
		writer.Flush ();

		input.text = "";
	}

	private void CloseSocket()
	{
		if (!socketReady)
			return;

		writer.Close ();
		reader.Close ();
		socket.Close ();
		socketReady = false;
	}

	private void OnApplicationQuit()
	{
		CloseSocket ();
	}

	private void OnDisable()
	{
		CloseSocket ();
	}
}
