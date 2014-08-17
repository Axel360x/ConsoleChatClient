using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace ClientSide
{
	public class Client
	{
		private string nickname = "";
		private static bool debug = false;
		[STAThread]
		public static void Main(){

			using(Mutex mutex = new Mutex(false, "Global\\" + appGuid))
			{
				if(!mutex.WaitOne(0, false) && !debug)
				{
					Console.WriteLine ("Already runnign");
					Console.Read ();
					return;
				}

				Client initClient = new Client ();
			}


		}
		private static string appGuid = "c0a76b5a-12ab-45c5-b9d9-d693faa6e7b9";
		public Client ()
		{
			try{
				Console.WriteLine("Enter your nickname");
                string[] ipconf = File.ReadAllLines("IPsetting.conf");
				while(nickname.Length < 3)
					nickname = Console.ReadLine().TrimEnd(new char[]{ (char)0 });
			TcpClient client = new TcpClient ();
				IPEndPoint serverEndPoint = new IPEndPoint (IPAddress.Parse(ipconf[0]), Convert.ToInt32(ipconf[1]));

			client.Connect (serverEndPoint);
			NetworkStream clientStream = client.GetStream ();
				ASCIIEncoding encoder = new ASCIIEncoding ();
				Console.WriteLine ("Connected to server from " + client.Client.LocalEndPoint + " as " + nickname + " using " + Environment.OSVersion.ToString());
				byte[] data = encoder.GetBytes(nickname + " " + Environment.OSVersion.ToString());
				clientStream.Write(data, 0, data.Length);
				clientStream.Flush();
			Thread getThread = new Thread (new ParameterizedThreadStart (getMessages));
			getThread.Start (client);

			while (true) {

					string message = nickname + ": " + Console.ReadLine ();
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					ClearCurrentConsoleLine();
				byte[] buffer = encoder.GetBytes (message);
				clientStream.Write (buffer, 0, buffer.Length);
				clientStream.Flush ();

			}
			}catch{
				Console.WriteLine ("404 NOT FOUND");
				Console.Read ();

			}


		}
		private void getMessages(object server)
		{
			TcpClient client = (TcpClient)server;
			NetworkStream stream = client.GetStream ();
			ASCIIEncoding encoder = new ASCIIEncoding ();
						byte[] message = new byte[4096];
						int bytesRead;
						while (true) {
			
							bytesRead = 0;
			
							try {
					message = new byte[4096];
					bytesRead = stream.Read (message, 0, message.Length);
								string text = encoder.GetString (message).TrimEnd (new char[]{ (char)0 });
					Console.WriteLine (text);
			
			
							} catch {
			
								break;
			
			
							}
							if (bytesRead == 0) {
			
								break;
			
							}
						}

		}
		public void ClearCurrentConsoleLine()
		{
			int currentLineCursor = Console.CursorTop;
			Console.SetCursorPosition(0, Console.CursorTop);
			for (int i = 0; i < Console.WindowWidth; i++)
				Console.Write(" ");
			Console.SetCursorPosition(0, currentLineCursor);
		}
	}
}

