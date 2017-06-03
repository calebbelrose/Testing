using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.IO;
using System.Net;

public class Server : MonoBehaviour
{
	private List<ServerClient> clients;
	private TcpListener server;
	private bool serverStarted;

	public int port = 6321;

	private void Start()
	{
		clients = new List<ServerClient> ();

		try
		{
			server = new TcpListener(IPAddress.Any, port);
			server.Start();

			StartListening();
			serverStarted = true;
			Debug.Log("Server has been started on port " + port.ToString());
		}
		catch(Exception e)
		{
			Debug.Log ("Socket error: " + e.Message);
		}
	}

	private void Update()
	{
		if (!serverStarted)
			return;

		int currClient = 0;

		while(currClient < clients.Count)
		{
			if (!IsConnected (clients[currClient].tcp))
			{
				clients[currClient].tcp.Close ();
				clients.RemoveAt (currClient);
				Broadcast (clients[currClient].clientName + " has disconnected", clients);
			}
			else
			{
				NetworkStream s = clients[currClient].tcp.GetStream ();
				if (s.DataAvailable)
				{
					StreamReader reader = new StreamReader (s, true);
					string data = reader.ReadLine ();

					if (data != null)
					{
						if (clients[currClient].clientName == "")
						{
							clients[currClient].clientName = data;
							Broadcast (clients[currClient].clientName + " has connected", clients);
						}
						else
							OnIncomingData (clients[currClient], data);
					}
				}
				currClient++;
			}
		}
	}

	private void StartListening()
	{
		server.BeginAcceptTcpClient(AcceptTcpClient, server);
	}

	private void AcceptTcpClient(IAsyncResult ar)
	{
		TcpListener listener = (TcpListener)ar.AsyncState;

		clients.Add (new ServerClient (listener.EndAcceptTcpClient(ar)));
		StartListening ();
	}

	private bool IsConnected(TcpClient c)
	{
		try
		{
			if(c != null && c.Client != null && c.Client.Connected)
			{
				if(c.Client.Poll(0, SelectMode.SelectRead))
					return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
				return true;
			}
			else
				return false;
		}
		catch
		{
			return false;
		}
	}

	private void OnIncomingData(ServerClient c, string data)
	{
		Broadcast (c.clientName + ": " + data, clients);
	}

	private void Broadcast(string data, List<ServerClient> cl)
	{
		foreach (ServerClient c in cl)
		{
			try
			{
				StreamWriter writer = new StreamWriter(c.tcp.GetStream());
				writer.WriteLine(data);
				writer.Flush();
			}
			catch(Exception e)
			{
				Debug.Log ("Write error: " + e.Message + " to client " + c.clientName);
			}
		}
	}
}

public class ServerClient
{
	public TcpClient tcp;
	public string clientName = "";

	public ServerClient(TcpClient clientSocket)
	{
		tcp = clientSocket;
	}
}
