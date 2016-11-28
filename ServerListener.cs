using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ProxyServer
{
	class ServerListener
	{
		private int listenPort;
		private TcpListener server;
		
		public ServerListener(int port)
		{
			string dhcpIpAddress = System.Net.Dns.GetHostAddresses(Environment.MachineName)[3].ToString();
			this.listenPort = port;
			this.server = new TcpListener(System.Net.IPAddress.Parse(dhcpIpAddress), this.listenPort);
		}

		public void StartServer()
		{
			this.server.Start();
		}

		public void AcceptConnection()
		{
			Socket newClient = this.server.AcceptSocket();
			ClientConnection client = new ClientConnection(newClient);
			client.StartHandling();
		}
	}
}
