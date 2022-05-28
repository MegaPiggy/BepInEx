using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BepInEx.OWMM
{

	public class ModSocket
	{
		private const int CloseWaitSeconds = 1;

		private Socket _socket;
		private readonly int _port;

		public ModSocket(int socketPort)
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_port = socketPort;
		}

		public void WriteToSocket(ModSocketMessage message)
		{
			if (_socket == null)
			{
				return;
			}
			if (!_socket.Connected)
			{
				Connect();
			}

			var json = message.ToJSON();
			var bytes = Encoding.UTF8.GetBytes(json + Environment.NewLine);
			try
			{
				_socket?.Send(bytes);
			}
			catch (SocketException) { }
		}

		public void Close()
		{
			Thread.Sleep(TimeSpan.FromSeconds(CloseWaitSeconds));
			_socket?.Close();
		}

		private void Connect()
		{
			var ipAddress = IPAddress.Parse("127.0.0.1");
			var endPoint = new IPEndPoint(ipAddress, _port);
			try
			{
				_socket?.Connect(endPoint);
			}
			catch (Exception)
			{
				_socket = null;
			}
		}
	}
}
