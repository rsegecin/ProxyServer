using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ProxyServer
{
	class ClientConnection
	{
		private Socket clientSocket;

		public ClientConnection(Socket client)
		{
			this.clientSocket = client;
		}

		public void StartHandling()
		{
			Thread handler = new Thread(Handler);
			handler.Priority = ThreadPriority.Normal;
			handler.Start();
		}

		private void Handler()
		{
			bool recvRequest = true;
			string EOL = "\r\n";

			string requestPayload = "";
			string requestTempLine = "";
			List<string> requestLines = new List<string>();
			byte[] requestBuffer = new byte[1];
			byte[] responseBuffer = new byte[1];

			requestLines.Clear();

			try
			{
				//State 0: Handle Request from Client
				while (recvRequest)
				{
					this.clientSocket.Receive(requestBuffer);
					string fromByte = ASCIIEncoding.ASCII.GetString(requestBuffer);
					
					requestPayload += fromByte;
					requestTempLine += fromByte;

					if (requestTempLine.EndsWith(EOL))
					{
						requestLines.Add(requestTempLine.Trim());
						requestTempLine = "";
					}

					if (requestPayload.EndsWith(EOL + EOL))
					{
						recvRequest = false;
					}
				}

				//Console.WriteLine("Raw Request Received...");
				Console.WriteLine();
				Console.WriteLine("=============================================");
				Console.WriteLine(requestPayload);

				//State 1: Rebuilding Request Information and Create Connection to Destination Server
				Uri url = new Uri(requestLines[0].Split(' ')[1]);

				//Socket destServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				Socket destServerSocket = new Socket(IPAddress.Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				if (url.Scheme.Equals("http"))
				{
					destServerSocket.Connect(url.Host, 80);
					Console.WriteLine("Trying to access " + url.Host);
				}
				else
				{
					destServerSocket.Connect(url.Scheme, 443);
					Console.WriteLine("Trying to access " + url.Scheme);
				}

				//State 2: Sending New Request Information to Destination Server and Relay Response to Client
				destServerSocket.Send(ASCIIEncoding.ASCII.GetBytes(requestPayload));

				//Console.WriteLine("Begin Receiving Response...");
				while (destServerSocket.Receive(responseBuffer) != 0)
				{
					//string r = ASCIIEncoding.ASCII.GetString(responseBuffer).Replace("google", "");
					this.clientSocket.Send(responseBuffer);
				}

				Console.WriteLine("Data Sent");
				//this.clientSocket.Send(Encoding.ASCII.GetBytes(" HELLO Rinaldi"));

				destServerSocket.Disconnect(false);
				destServerSocket.Dispose();
				this.clientSocket.Disconnect(false);
				this.clientSocket.Dispose();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error Occured: " + e.Message);
				//Console.WriteLine(e.StackTrace);
			}
		}
	}
}
