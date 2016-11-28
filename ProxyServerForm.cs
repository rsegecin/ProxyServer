using System.Windows.Forms;

namespace ProxyServer
{
	public partial class ProxyServerForm : Form
	{
		public ProxyServerForm()
		{
			InitializeComponent();

			ServerListener simpleHttpProxyServer = new ServerListener(9000);
			simpleHttpProxyServer.StartServer();
			while (true)
			{
				simpleHttpProxyServer.AcceptConnection();
			}
		}
	}
}
