namespace Networking
{
	using System;
	using UnityEngine;
	using UnityEngine.Networking;

	public class ServerTypeLoader : NetworkManager
	{
		void Start()
		{
			networkAddress = "97e17342.skybroadband.com";
			networkPort = 7777;

			string commandLineOptions = Environment.CommandLine;

			if (commandLineOptions.Contains("-batchmode"))
			{
				StartHost();
				{
					Debug.Log("Started Server at - " + networkPort);
				}
			}
			else
			{
				StartClient();
			}
		}
	}
}
